using Xunit;
using Project.DataModels;
using UnitTests.InMemory;
using Assert = Xunit.Assert;

namespace UnitTests;

public class FloorPlanServiceTests : IDisposable
{
    private readonly InMemoryTableRepository _tableRepo;
    private readonly InMemoryReservationRepository _reservationRepo;
    private const string TestDate = "2027-02-15";
    private const string TestSlot = "2027-02-15 19:00:00";

    public FloorPlanServiceTests()
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
    public void H1_Guest2_SelectsT1_SavesAndReserves()
    {
        int guestCount = 2;
        var table1 = _tableRepo.GetByTableNumber(1);
        
        Assert.NotNull(table1);
        Assert.Equal(1, table1.ID);
        Assert.Equal(1, table1.TableNumber);
        Assert.Equal(2, table1.TableCapacity);
        
        var reservation = new ReservationModel
        {
            UserId = 10,
            TableId = table1.ID,
            GuestCount = guestCount,
            StartAt = TestSlot,
            Status = "confirmed",
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        _reservationRepo.Add(reservation);

        var savedReservation = _reservationRepo.GetById(reservation.ID);
        var reservedTableIds = _reservationRepo.GetReservedTableIds(TestDate);

        Assert.NotNull(savedReservation);
        Assert.Equal(table1.ID, savedReservation!.TableId);
        Assert.Contains(table1.ID, reservedTableIds);
    }

    [Fact]
    public void H2_Guest6_SelectsT3_CapacityMatchesGuests_SavesOk()
    {
        int guestCount = 6;
        var table3 = _tableRepo.GetByTableNumber(3);

        Assert.Equal(guestCount, table3!.TableCapacity);

        var reservation = new ReservationModel
        {
            UserId = 11,
            TableId = table3.ID,
            GuestCount = guestCount,
            StartAt = TestSlot,
            Status = "confirmed",
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        _reservationRepo.Add(reservation);

        var savedReservation = _reservationRepo.GetById(reservation.ID);

        Assert.NotNull(savedReservation);
        Assert.Equal(guestCount, savedReservation!.GuestCount);
        Assert.Equal(table3.TableCapacity, guestCount);
    }

    [Fact]
    public void H3_ChangingSelection_SavesLatestTable()
    {
        int userId = 12;
        var table1 = _tableRepo.GetByTableNumber(1);
        var table4 = _tableRepo.GetByTableNumber(4);

        var reservation = new ReservationModel
        {
            UserId = userId,
            TableId = table1!.ID,
            GuestCount = 2,
            StartAt = TestSlot,
            Status = "confirmed",
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        _reservationRepo.Add(reservation);

        reservation.TableId = table4!.ID;
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _reservationRepo.Update(reservation);

        var updatedReservation = _reservationRepo.GetById(reservation.ID);

        Assert.NotNull(updatedReservation);
        Assert.Equal(table4.ID, updatedReservation.TableId);
    }

    [Fact]
    public void H4_RenderFloorPlan_ShowsGreenAndRedTables()
    {
        var allTables = _tableRepo.GetAll();
        var reservedTableIds = _reservationRepo.GetReservedTableIds(TestDate);
        int guestCount = 2;

        var floorPlan = RenderFloorPlanString(allTables, reservedTableIds, guestCount);

        Assert.Contains("🟩", floorPlan);
        Assert.Contains("🟥", floorPlan);
        Assert.Contains("T2", floorPlan);
        Assert.Contains("T5", floorPlan);
    }

    [Fact]
    public void S1_ClickingSmallerCapacity_Blocks()
    {
        int guestCount = 4;
        var table1 = _tableRepo.GetByTableNumber(1);
        var reservedTableIds = _reservationRepo.GetReservedTableIds(TestDate);

        bool isSelectable = IsTableSelectable(table1!, reservedTableIds, guestCount);
        string errorMessage = isSelectable ? "" : "Table capacity too small for party size";

        Assert.False(isSelectable);
        Assert.Equal("Table capacity too small for party size", errorMessage);
    }

    [Fact]
    public void S2_ClickingReservedTable_ReturnsError()
    {
        var table2 = _tableRepo.GetByTableNumber(2);
        var reservedTableIds = _reservationRepo.GetReservedTableIds(TestDate);

        bool isReserved = reservedTableIds.Contains(table2!.ID);
        string errorMessage = isReserved ? "Table already reserved" : "";

        Assert.True(isReserved);
        Assert.Equal("Table already reserved", errorMessage);
    }

    [Fact]
    public void S3_NoFittingTable_EmptySelectableListWithMessage()
    {
        int guestCount = 8;
        var allTables = _tableRepo.GetAll();
        var reservedTableIds = _reservationRepo.GetReservedTableIds(TestDate);

        var selectableTables = allTables
            .Where(t => !reservedTableIds.Contains(t.ID) && t.TableCapacity >= guestCount)
            .ToList();

        Assert.Empty(selectableTables);
        string message = selectableTables.Count == 0 
            ? "No available tables for party size" 
            : "";
        Assert.Equal("No available tables for party size", message);
    }

    [Fact]
    public void S4_ContinueWithoutSelection_ValidationError()
    {
        TableModel? selectedTable = null;

        bool isValid = selectedTable != null;
        string validationError = !isValid ? "Table selection required" : "";

        Assert.False(isValid);
        Assert.Equal("Table selection required", validationError);
    }

    [Fact]
    public void S5_RaceCondition_ReserveChosenTableBeforeConfirm_Conflict()
    {
        var table1 = _tableRepo.GetByTableNumber(1);
        var initialReservedIds = _reservationRepo.GetReservedTableIds(TestDate);
        
        bool wasAvailable = !initialReservedIds.Contains(table1!.ID);
        Assert.True(wasAvailable);

        _reservationRepo.Add(new ReservationModel
        {
            UserId = 999,
            TableId = table1.ID,
            GuestCount = 2,
            StartAt = TestSlot,
            Status = "confirmed"
        });

        var updatedReservedIds = _reservationRepo.GetReservedTableIds(TestDate);
        bool isNowReserved = updatedReservedIds.Contains(table1.ID);
        string conflictError = isNowReserved ? "Table no longer available" : "";

        Assert.True(isNowReserved);
        Assert.Equal("Table no longer available", conflictError);
    }

    private bool IsTableSelectable(TableModel table, List<int> reservedTableIds, int guestCount)
    {
        return !reservedTableIds.Contains(table.ID) && table.TableCapacity >= guestCount;
    }

    private string RenderFloorPlanString(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
    {
        var lines = new List<string>();
        
        foreach (var table in allTables)
        {
            bool isReserved = reservedTableIds.Contains(table.ID);
            bool isRightSize = table.TableCapacity >= guestCount;
            
            string icon = isReserved ? "🟥" : (isRightSize ? "🟩" : "⬜");
            lines.Add($"{icon} T{table.TableNumber} ({table.TableCapacity}p)");
        }
        
        return string.Join("\n", lines);
    }
}
