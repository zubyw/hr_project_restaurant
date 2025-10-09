using Microsoft.Data.Sqlite;
using Project.DataModels;
using Dapper;

public class ReservationsAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db";
    private readonly string Table = "Reservations";

    public void Write(ReservationModel reservation)
    {
        string sql = $"INSERT INTO {Table} (UserId, TableId, GuestCount, StartAt, Status, CreatedAt, UpdatedAt) VALUES (@UserId, @TableId, @GuestCount, @StartAt, @Status, @CreatedAt, @UpdatedAt)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, reservation);
    }

    public ReservationModel? GetById(int id)
    {
        string sql = @"
            SELECT r.*, u.FirstName as GuestFirstName, u.LastName as GuestLastName, u.EmailAddress as GuestEmail,
                   t.TableNumber, t.TableCapacity 
            FROM Reservations r 
            JOIN Users u ON r.UserId = u.ID 
            JOIN [Table] t ON r.TableId = t.ID 
            WHERE r.ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<ReservationModel>(sql, new { Id = id });
    }

    public List<ReservationModel> GetAll()
    {
        string sql = @"
            SELECT r.*, u.FirstName as GuestFirstName, u.LastName as GuestLastName, u.EmailAddress as GuestEmail,
                   t.TableNumber, t.TableCapacity 
            FROM Reservations r 
            JOIN Users u ON r.UserId = u.ID 
            JOIN [Table] t ON r.TableId = t.ID 
            ORDER BY r.StartAt";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<ReservationModel>(sql).ToList();
    }

    public List<ReservationModel> GetByDate(string date)
    {
        string sql = @"
            SELECT r.*, u.FirstName as GuestFirstName, u.LastName as GuestLastName, u.EmailAddress as GuestEmail,
                   t.TableNumber, t.TableCapacity 
            FROM Reservations r 
            JOIN Users u ON r.UserId = u.ID 
            JOIN [Table] t ON r.TableId = t.ID 
            WHERE DATE(r.StartAt) = @Date 
            ORDER BY r.StartAt";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<ReservationModel>(sql, new { Date = date }).ToList();
    }

    public List<ReservationModel> GetByDateRange(string startDate, string endDate)
    {
        string sql = @"
            SELECT r.*, u.FirstName as GuestFirstName, u.LastName as GuestLastName, u.EmailAddress as GuestEmail,
                   t.TableNumber, t.TableCapacity 
            FROM Reservations r 
            JOIN Users u ON r.UserId = u.ID 
            JOIN [Table] t ON r.TableId = t.ID 
            WHERE DATE(r.StartAt) BETWEEN @StartDate AND @EndDate 
            ORDER BY r.StartAt";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<ReservationModel>(sql, new { StartDate = startDate, EndDate = endDate }).ToList();
    }

    public List<ReservationModel> GetByUserId(int userId)
    {
        string sql = @"
            SELECT r.*, u.FirstName as GuestFirstName, u.LastName as GuestLastName, u.EmailAddress as GuestEmail,
                   t.TableNumber, t.TableCapacity 
            FROM Reservations r 
            JOIN Users u ON r.UserId = u.ID 
            JOIN [Table] t ON r.TableId = t.ID 
            WHERE r.UserId = @UserId 
            ORDER BY r.StartAt";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<ReservationModel>(sql, new { UserId = userId }).ToList();
    }

    public void Update(ReservationModel reservation)
    {
        string sql = $"UPDATE {Table} SET UserId = @UserId, TableId = @TableId, GuestCount = @GuestCount, StartAt = @StartAt, Status = @Status, UpdatedAt = @UpdatedAt WHERE ID = @ID";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, reservation);
    }

    public void Delete(ReservationModel reservation)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @ID";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { ID = reservation.ID });
    }

    public void DeleteById(int id)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { Id = id });
    }
}
