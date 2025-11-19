using Xunit;
using Project.DataModels;
using UnitTests.InMemory;
using Assert = Xunit.Assert;

namespace UnitTests;

public class ReservationServiceTests : IDisposable
{
    private readonly InMemoryTableRepository _tableRepo;
    private readonly InMemoryReservationRepository _reservationRepo;
    private const string TestDate = "15-02-2027";
    private const string TestSlot = "15-02-2027 19:00:00";

    public ReservationServiceTests()
    {
        _tableRepo = new InMemoryTableRepository();
        _reservationRepo = new InMemoryReservationRepository();
        
        SetupTestData();
    }

    public void Dispose()
    {
        _tableRepo.Clear();
        _reservationRepo.Clear();
    }

    private void SetupTestData()
    {
        _tableRepo.Add(new TableModel { ID = 1, TableNumber = 1, TableCapacity = 2 });
        _tableRepo.Add(new TableModel { ID = 2, TableNumber = 2, TableCapacity = 4 });
        _tableRepo.Add(new TableModel { ID = 3, TableNumber = 3, TableCapacity = 6 });
        _tableRepo.Add(new TableModel { ID = 4, TableNumber = 4, TableCapacity = 2 });
        _tableRepo.Add(new TableModel { ID = 5, TableNumber = 5, TableCapacity = 4 });
        _tableRepo.Add(new TableModel { ID = 6, TableNumber = 6, TableCapacity = 6 });

        _reservationRepo.Add(new ReservationModel
        {
            ID = 1,
            UserId = 1,
            TableId = 2,
            GuestCount = 4,
            StartAt = TestSlot,
            Status = "confirmed"
        });

        _reservationRepo.Add(new ReservationModel
        {
            ID = 2,
            UserId = 2,
            TableId = 5,
            GuestCount = 3,
            StartAt = TestSlot,
            Status = "confirmed"
        });
    }

    [Fact]
    public void CreateReservation_WithValidTable_Success()
    {
        var table = _tableRepo.GetByTableNumber(1);
        
        var reservation = new ReservationModel
        {
            UserId = 10,
            TableId = table!.ID,
            GuestCount = 2,
            StartAt = TestSlot,
            Status = "confirmed",
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        
        _reservationRepo.Add(reservation);
        var result = _reservationRepo.GetById(reservation.ID);

        Assert.NotNull(result);
        Assert.Equal(table.ID, result!.TableId);
        Assert.Equal(2, result.GuestCount);
    }

    [Fact]
    public void UpdateReservation_ChangeTable_Success()
    {
        var oldTable = _tableRepo.GetByTableNumber(1);
        var newTable = _tableRepo.GetByTableNumber(4);
        
        var reservation = new ReservationModel
        {
            UserId = 11,
            TableId = oldTable!.ID,
            GuestCount = 2,
            StartAt = TestSlot,
            Status = "confirmed",
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        _reservationRepo.Add(reservation);

        reservation.TableId = newTable!.ID;
        reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        _reservationRepo.Update(reservation);
        
        var updated = _reservationRepo.GetById(reservation.ID);

        Assert.NotNull(updated);
        Assert.Equal(newTable.ID, updated!.TableId);
    }

    [Fact]
    public void GetReservationsByDate_FiltersCorrectly()
    {
        var reservations = _reservationRepo.GetByDate(TestDate);

        Assert.Equal(2, reservations.Count);
        Assert.All(reservations, r => Assert.StartsWith(TestDate, r.StartAt));
    }

    [Fact]
    public void GetReservedTableIds_ReturnsCorrectTables()
    {
        var reservedIds = _reservationRepo.GetReservedTableIds(TestDate);

        Assert.Contains(2, reservedIds);
        Assert.Contains(5, reservedIds);
        Assert.Equal(2, reservedIds.Count);
    }

    [Fact]
    public void ValidateReservation_TableTooSmall_Fails()
    {
        var table = _tableRepo.GetByTableNumber(1);
        int guestCount = 4;

        bool isValid = table!.TableCapacity >= guestCount;

        Assert.False(isValid);
    }

    [Fact]
    public void ValidateReservation_TableAlreadyReserved_Fails()
    {
        var table = _tableRepo.GetByTableNumber(2);
        var reservedIds = _reservationRepo.GetReservedTableIds(TestDate);

        bool isAvailable = !reservedIds.Contains(table!.ID);

        Assert.False(isAvailable);
    }

    [Fact]
    public void ValidateReservation_ValidTableAndCapacity_Success()
    {
        var table = _tableRepo.GetByTableNumber(6);
        var reservedIds = _reservationRepo.GetReservedTableIds(TestDate);
        int guestCount = 4;

        bool isAvailable = !reservedIds.Contains(table!.ID);
        bool hasCapacity = table.TableCapacity >= guestCount;
        bool isValid = isAvailable && hasCapacity;

        Assert.True(isValid);
    }

    [Fact]
    public void DeleteReservation_RemovesSuccessfully()
    {
        var table = _tableRepo.GetByTableNumber(1);
        var reservation = new ReservationModel
        {
            UserId = 12,
            TableId = table!.ID,
            GuestCount = 2,
            StartAt = TestSlot,
            Status = "confirmed"
        };
        _reservationRepo.Add(reservation);
        
        var reservedBefore = _reservationRepo.GetReservedTableIds(TestDate);
        Assert.Contains(table.ID, reservedBefore);

        _reservationRepo.Delete(reservation.ID);
        
        var reservedAfter = _reservationRepo.GetReservedTableIds(TestDate);
        var deleted = _reservationRepo.GetById(reservation.ID);

        Assert.Null(deleted);
        Assert.DoesNotContain(table.ID, reservedAfter);
    }
}
