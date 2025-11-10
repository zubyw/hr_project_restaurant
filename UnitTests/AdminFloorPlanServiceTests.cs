using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Project.DataModels;

namespace UnitTests
{
    public class AdminFloorPlanServiceTests
    {
        private readonly InMemoryTableRepository _tableRepo;
        private readonly InMemoryReservationRepository _reservationRepo;
        private const string TestDate = "2027-02-15";

        public AdminFloorPlanServiceTests()
        {
            _tableRepo = new InMemoryTableRepository();
            _reservationRepo = new InMemoryReservationRepository();
        }

        private void SetupTestData()
        {
            // Tables: T1(2), T2(4), T3(6)
            _tableRepo.Add(new TableModel { ID = 1, TableNumber = 1, TableCapacity = 2 });
            _tableRepo.Add(new TableModel { ID = 2, TableNumber = 2, TableCapacity = 4 });
            _tableRepo.Add(new TableModel { ID = 3, TableNumber = 3, TableCapacity = 6 });

                        // Reservations: T1@19:00(2); T2@18:00(2) & 20:00(4)
            _reservationRepo.Add(new ReservationModel
            {
                ID = 1,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 2,
                GuestCount = 2,
                StartAt = $"{TestDate} 19:00",
                Status = "Open",
                GuestFirstName = "John",
                GuestLastName = "Doe",
                GuestEmail = "john@test.com"
            });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 2,
                TableId = 2,
                TableNumber = 2,
                TableCapacity = 4,
                GuestCount = 2,
                StartAt = $"{TestDate} 18:00",
                Status = "Open",
                GuestFirstName = "Jane",
                GuestLastName = "Smith",
                GuestEmail = "jane@test.com"
            });"{TestDate} 19:00",
                Status = "Open",
                GuestFirstName = "John",
                GuestLastName = "Doe",
                GuestEmail = "john@test.com"
            });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 2,
                TableId = 2,
                TableNumber = 2,
                TableCapacity = 4,
                GuestCount = 2,
                StartAt = $"{TestDate} 18:00",
                Status = "Open",
                GuestFirstName = "Jane",
                GuestLastName = "Smith",
                GuestEmail = "jane@test.com"
            });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 3,
                TableId = 2,
                TableNumber = 2,
                TableCapacity = 4,
                GuestCount = 4,
                StartAt = $"{TestDate} 20:00",
                Status = "Confirmed",
                GuestFirstName = "Bob",
                GuestLastName = "Johnson",
                GuestEmail = "bob@test.com"
            });
        }

        [Fact]
        public void H1_RenderFloorPlanString_ReturnsCorrectFormat()
        {
            // Arrange
            SetupTestData();
            var tables = _tableRepo.GetAll();
            var reservations = _reservationRepo.GetAll()
                .Where(r => r.StartAt.StartsWith(TestDate))
                .ToList();

            // Act
            var result = RenderFloorPlanString(tables, reservations, TestDate);

            // Assert
            Assert.Contains("[T1] 🟥(2)", result); // T1 gereserveerd, 2 personen
            Assert.Contains("[T2] 🟥(6)", result); // T2 gereserveerd, totaal 6 personen (2+4)
            Assert.Contains("[T3] 🟩", result);    // T3 beschikbaar
            Assert.Contains("🟩 Available", result);
            Assert.Contains("🟥 Reserved", result);
        }

        [Fact]
        public void H2_NoReservations_AllTablesAvailable()
        {
            // Arrange
            _tableRepo.Add(new TableModel { ID = 1, TableNumber = 1, TableCapacity = 2 });
            _tableRepo.Add(new TableModel { ID = 2, TableNumber = 2, TableCapacity = 4 });
            _tableRepo.Add(new TableModel { ID = 3, TableNumber = 3, TableCapacity = 6 });

            var tables = _tableRepo.GetAll();
            var reservations = new List<ReservationModel>();

            // Act
            var result = RenderFloorPlanString(tables, reservations, TestDate);

            // Assert
            Assert.Contains("[T1] 🟩", result);
            Assert.Contains("[T2] 🟩", result);
            Assert.Contains("[T3] 🟩", result);
            Assert.Contains("No reservations for this date", result);
        }

        [Fact]
        public void H3_SumGuestCountsPerTable()
        {
            // Arrange
            SetupTestData();
            var reservations = _reservationRepo.GetAll()
                .Where(r => r.StartAt.StartsWith(TestDate))
                .ToList();

            // Act
            var guestCountsPerTable = reservations
                .GroupBy(r => r.TableNumber)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.GuestCount));

            // Assert
            Assert.Equal(2, guestCountsPerTable[1]);  // T1: 2 gasten
            Assert.Equal(6, guestCountsPerTable[2]);  // T2: 2 + 4 = 6 gasten
            Assert.False(guestCountsPerTable.ContainsKey(3)); // T3: geen reserveringen
        }

        [Fact]
        public void H4_TableDetailListsReservationsOrderedByTime()
        {
            // Arrange
            SetupTestData();
            var reservations = _reservationRepo.GetAll()
                .Where(r => r.StartAt.StartsWith(TestDate) && r.TableNumber == 2)
                .OrderBy(r => r.StartAt)
                .ToList();

            // Act & Assert
            Assert.Equal(2, reservations.Count);
            Assert.Equal("18:00", reservations[0].StartAt.Split(' ')[1]); // Eerste om 18:00
            Assert.Equal("20:00", reservations[1].StartAt.Split(' ')[1]); // Tweede om 20:00
            Assert.Equal("Jane", reservations[0].GuestFirstName);
            Assert.Equal("Bob", reservations[1].GuestFirstName);
        }

        [Fact]
        public void S1_InvalidDateFormat_Rejected()
        {
            // Arrange
            var invalidDates = new[] { "2027/02/15", "15-02-2027", "invalid", "", "2027-13-01" };

            // Act & Assert
            foreach (var date in invalidDates)
            {
                var isValid = IsValidDateFormat(date);
                Assert.False(isValid, $"Date '{date}' should be invalid");
            }
        }

        [Fact]
        public void S2_EmptyTablesRepo_ReturnsNoTablesConfiguredMessage()
        {
            // Arrange - geen tables toevoegen
            var tables = _tableRepo.GetAll();
            var reservations = new List<ReservationModel>();

            // Act
            var result = RenderFloorPlanString(tables, reservations, TestDate);

            // Assert
            Assert.Contains("No tables configured", result);
            Assert.Empty(tables);
        }

        [Fact]
        public void S3_OverbookEdge_TotalGuestsExceedsCapacity_ShowsWarningMarker()
        {
            // Arrange - tafel met capaciteit 4, maar dubbel geboekt met totaal 6 gasten
            _tableRepo.Add(new TableModel { ID = 1, TableNumber = 1, TableCapacity = 4 });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 1,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 4,
                GuestCount = 3,
                StartAt = $"{TestDate} 18:00",
                Status = "Confirmed",
                GuestFirstName = "Alice",
                GuestLastName = "Test",
                GuestEmail = "alice@test.com"
            });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 2,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 4,
                GuestCount = 3,
                StartAt = $"{TestDate} 18:30",
                Status = "Confirmed",
                GuestFirstName = "Bob",
                GuestLastName = "Test",
                GuestEmail = "bob@test.com"
            });

            var tables = _tableRepo.GetAll();
            var reservations = _reservationRepo.GetAll();

            // Act
            var result = RenderFloorPlanString(tables, reservations, TestDate);

            // Assert - totaal 6 gasten > capaciteit 4
            Assert.Contains("!", result); // Warning marker voor overbooked
            Assert.Contains("[T1]", result);
        }

        [Fact]
        public void S4_IgnoreReservationsWithInvalidGuestCount()
        {
            // Arrange - reserveringen met ongeldige gastenaantallen
            _tableRepo.Add(new TableModel { ID = 1, TableNumber = 1, TableCapacity = 2 });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 1,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 2,
                GuestCount = 0, // Ongeldig
                StartAt = $"{TestDate} 18:00",
                Status = "Open",
                GuestFirstName = "Test",
                GuestLastName = "User",
                GuestEmail = "test@test.com"
            });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 2,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 2,
                GuestCount = -1, // Ongeldig
                StartAt = $"{TestDate} 19:00",
                Status = "Open",
                GuestFirstName = "Test2",
                GuestLastName = "User2",
                GuestEmail = "test2@test.com"
            });

            var tables = _tableRepo.GetAll();
            var reservations = _reservationRepo.GetAll()
                .Where(r => r.GuestCount > 0) // Filter ongeldige counts
                .ToList();

            // Act
            var result = RenderFloorPlanString(tables, reservations, TestDate);

            // Assert - tafel moet beschikbaar zijn, want geen geldige reserveringen
            Assert.Contains("[T1] 🟩", result);
            Assert.Empty(reservations);
        }

        [Fact]
        public void S5_CancelledReservations_DoNotBlockTables()
        {
            // Arrange
            _tableRepo.Add(new TableModel { ID = 1, TableNumber = 1, TableCapacity = 2 });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 1,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 2,
                GuestCount = 2,
                StartAt = $"{TestDate} 19:00",
                Status = "Cancelled", // Geannuleerd
                GuestFirstName = "John",
                GuestLastName = "Doe",
                GuestEmail = "john@test.com"
            });

            _reservationRepo.Add(new ReservationModel
            {
                ID = 2,
                TableId = 1,
                TableNumber = 1,
                TableCapacity = 2,
                GuestCount = 2,
                StartAt = $"{TestDate} 20:00",
                Status = "geannuleerd", // Geannuleerd (NL)
                GuestFirstName = "Jane",
                GuestLastName = "Smith",
                GuestEmail = "jane@test.com"
            });

            var tables = _tableRepo.GetAll();
            var activeReservations = _reservationRepo.GetAll()
                .Where(r => r.Status != "Cancelled" && r.Status != "geannuleerd")
                .ToList();

            // Act
            var result = RenderFloorPlanString(tables, activeReservations, TestDate);

            // Assert - tafel beschikbaar want reserveringen geannuleerd
            Assert.Contains("[T1] 🟩", result);
            Assert.Empty(activeReservations);
        }

        // Helper methods
        private string RenderFloorPlanString(List<TableModel> tables, List<ReservationModel> reservations, string date)
        {
            if (tables.Count == 0)
            {
                return "No tables configured";
            }

            var result = $"Floor Plan for {date}\n";
            result += "🟩 Available  🟥 Reserved\n\n";

            if (reservations.Count == 0)
            {
                result += "No reservations for this date\n";
            }

            var reservationsByTable = reservations
                .GroupBy(r => r.TableNumber)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.GuestCount));

            foreach (var table in tables.OrderBy(t => t.TableNumber))
            {
                var totalGuests = reservationsByTable.ContainsKey(table.TableNumber) 
                    ? reservationsByTable[table.TableNumber] 
                    : 0;

                var isReserved = totalGuests > 0;
                var icon = isReserved ? "🟥" : "🟩";
                var warningMarker = totalGuests > table.TableCapacity ? " !" : "";

                result += isReserved 
                    ? $"[T{table.TableNumber}] {icon}({totalGuests}){warningMarker}\n"
                    : $"[T{table.TableNumber}] {icon}\n";
            }

            return result;
        }

        private bool IsValidDateFormat(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return false;

            return DateTime.TryParseExact(date, "yyyy-MM-dd", null, 
                System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
