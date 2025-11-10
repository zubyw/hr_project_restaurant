using Microsoft.Data.Sqlite;
using Project.DataModels;
using Dapper;

public class ReservationsAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db;Foreign Keys=False";
    private readonly string Table = "Reservations";

    public void Write(ReservationModel reservation)
    {
        string sql = $"INSERT INTO {Table} (UserId, TableId, GuestCount, StartAt, Status, CanModifyUntil, CreatedAt, UpdatedAt) VALUES (@UserId, @TableId, @GuestCount, @StartAt, @Status, @CanModifyUntil, @CreatedAt, @UpdatedAt)";
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

    // vanaf hier heb ik het uitgebreidt, ik heb nu de datamodels gebruikt om de arguments te gebruiken:

public bool IsTableFree(ReservationModel reservation)
{
    string sql = "SELECT COUNT(*) FROM Reservations WHERE TableId=@TableId AND StartAt=@StartAt AND ID!=@ID";
    using var connection = new SqliteConnection(_connectionString);
    int count = connection.ExecuteScalar<int>(sql, new { reservation.TableId, reservation.StartAt, reservation.ID });
    return count == 0;
}

public List<TableModel> GetFreeTables(ReservationModel reservation)
{
    string sql = @"SELECT * FROM [Table] 
                   WHERE TableCapacity>=@GuestCount 
                     AND ID NOT IN (SELECT TableId FROM Reservations WHERE StartAt=@StartAt AND Status!='geannuleerd')";
    using var connection = new SqliteConnection(_connectionString);
    return connection.Query<TableModel>(sql, new { reservation.GuestCount, reservation.StartAt }).AsList();
}

public List<TableModel> GetFreeTablesExcluding(ReservationModel reservation)
{
    string sql = @"SELECT * FROM [Table] 
                   WHERE TableCapacity>=@GuestCount 
                     AND ID NOT IN (SELECT TableId FROM Reservations WHERE StartAt=@StartAt AND Status!='geannuleerd' AND ID!=@ID)";
    using var connection = new SqliteConnection(_connectionString);
    return connection.Query<TableModel>(sql, new { reservation.GuestCount, reservation.StartAt, reservation.ID }).AsList();
}

public bool UpdateGuestCount(ReservationModel reservation)
{
    string sql = "UPDATE Reservations SET GuestCount=@GuestCount, UpdatedAt=CURRENT_TIMESTAMP WHERE ID=@ID";
    using var connection = new SqliteConnection(_connectionString);
    return connection.Execute(sql, new { reservation.GuestCount, reservation.ID }) > 0;
}

public bool UpdateReservationTime(ReservationModel reservation)
{
    string sql = "UPDATE Reservations SET StartAt=@StartAt, UpdatedAt=CURRENT_TIMESTAMP WHERE ID=@ID";
    using var connection = new SqliteConnection(_connectionString);
    return connection.Execute(sql, new { reservation.StartAt, reservation.ID }) > 0;
}

public bool UpdateReservationTable(ReservationModel reservation)
{
    string sql = "UPDATE Reservations SET TableId=@TableId, GuestCount=@GuestCount, UpdatedAt=CURRENT_TIMESTAMP WHERE ID=@ID";
    using var connection = new SqliteConnection(_connectionString);
    return connection.Execute(sql, new { reservation.TableId, reservation.GuestCount, reservation.ID }) > 0;
}

public bool CancelReservation(ReservationModel reservation)
{
    string sql = "UPDATE Reservations SET Status='geannuleerd', UpdatedAt=CURRENT_TIMESTAMP WHERE ID=@ID";
    using var connection = new SqliteConnection(_connectionString);
    return connection.Execute(sql, new { reservation.ID }) > 0;
}


    // Methods from RudReservationsAccess (merged for consolidation)
    public List<ReservationModel> GetReservationsByUserIdSimple(int userId)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string query = "SELECT * FROM Reservations WHERE UserId = @UserId";
            List<ReservationModel> list = connection.Query<ReservationModel>(query, new { UserId = userId }).AsList();
            return list;
        }
    }

    public void UpdateReservationSimple(int id, int guestCount, string startAt)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string query = "UPDATE Reservations SET GuestCount = @GuestCount, StartAt = @StartAt, UpdatedAt = datetime('now') WHERE Id = @Id";
            connection.Execute(query, new
            {
                GuestCount = guestCount,
                StartAt = startAt,
                Id = id
            });
        }
    }

    public void DeleteReservationSimple(int id)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string query = "DELETE FROM Reservations WHERE Id = @Id";
            connection.Execute(query, new { Id = id });
        }
    }

    public ReservationModel? GetLatestReservationByUserId(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"
                SELECT * 
                FROM Reservations 
                WHERE UserId = @UserId 
                ORDER BY CreatedAt DESC 
                LIMIT 1";

            return connection.QueryFirstOrDefault<ReservationModel>(query, new { UserId = userId });
        }

}
