using Microsoft.Data.Sqlite;
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

    // vanaf hier heb ik het uitgebreidt:

        public bool IsTableFree(string dateTime, int tableId, int excludeReservationId)
    {
        string sql = $"SELECT COUNT(*) FROM Reservations WHERE TableId = @TableId AND StartAt = @StartAt AND ID != @ReservationId";
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        int count = connection.ExecuteScalar<int>(sql, new { TableId = tableId, StartAt = dateTime, ReservationId = excludeReservationId });
        connection.Close();
        return count == 0;
    }

    public List<TableModel> GetFreeTables(string dateTime, int persons)
    {
        string sql = "SELECT * FROM [Table] WHERE TableCapacity >= @Persons AND ID NOT IN (SELECT TableId FROM Reservations WHERE StartAt = @StartAt)";
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        List<TableModel> tables = connection.Query<TableModel>(sql, new { Persons = persons, StartAt = dateTime }).AsList();
        connection.Close();
        return tables;
    }

    public bool UpdateReservationTime(int reservationId, string newTime)
    {
        string sql = "UPDATE Reservations SET StartAt = @NewTime, UpdatedAt = CURRENT_TIMESTAMP WHERE ID = @ReservationId";
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        int rows = connection.Execute(sql, new { NewTime = newTime, ReservationId = reservationId });
        connection.Close();
        return rows > 0;
    }

    public bool UpdateReservationTable(int reservationId, int newTableId, int newPersons)
    {
        string sql = "UPDATE Reservations SET TableId = @TableId, GuestCount = @Persons, UpdatedAt = CURRENT_TIMESTAMP WHERE ID = @ReservationId";
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        int rows = connection.Execute(sql, new { TableId = newTableId, Persons = newPersons, ReservationId = reservationId });
        connection.Close();
        return rows > 0;
    }

    public bool CancelReservation(int reservationId)
    {
        string sql = "UPDATE Reservations SET Status = 'geannuleerd', UpdatedAt = CURRENT_TIMESTAMP WHERE ID = @ReservationId";
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        int rows = connection.Execute(sql, new { ReservationId = reservationId });
        connection.Close();
        return rows > 0;
    }



}
