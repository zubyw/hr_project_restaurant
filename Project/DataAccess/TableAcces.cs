using Microsoft.Data.Sqlite;
using Dapper;

public class TableAcces
{
    private static readonly string _connectionString = "Data Source=DataSources/project.db";


    public TableModel? GetAvailableTable(string reservationDate, int tablesize)
{
    // Get all existing tables in the restaurant
    string allTablesSql = @"
        SELECT t.ID, t.TableNumber, t.TableCapacity
        FROM [Table] t";
    using var connection = new SqliteConnection(_connectionString);
    List<TableModel> allTables = connection.Query<TableModel>(allTablesSql).ToList();

    // Get non available tables for the given date
    string reservedTablesSql = @"
        SELECT t.ID
        FROM Reservations r
        JOIN [Table] t ON r.TableId = t.ID
        WHERE DATE(r.StartAt) = @ReservationDate";
    List<int> reservedTableIds = connection.Query<int>(reservedTablesSql, new { ReservationDate = reservationDate }).ToList();

    // Filter wich tables are available 
    List<TableModel> availableTables = allTables.Where(t => !reservedTableIds.Contains(t.ID)).ToList();

    // Handle case where no tables are available
    if (!availableTables.Any())
    {
        Console.WriteLine("No available tables found.");
        return null;
    }

    // Find a table that fits the tablesize
    return availableTables.FirstOrDefault(t => t.TableCapacity >= tablesize);
}
}