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
            WHERE substr(r.StartAt, 1, 10) = @Date 
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
            WHERE substr(r.StartAt, 7, 4) || substr(r.StartAt, 4, 2) || substr(r.StartAt, 1, 2) 
                  BETWEEN substr(@StartDate, 7, 4) || substr(@StartDate, 4, 2) || substr(@StartDate, 1, 2)
                  AND substr(@EndDate, 7, 4) || substr(@EndDate, 4, 2) || substr(@EndDate, 1, 2)
            ORDER BY substr(r.StartAt, 7, 4) || substr(r.StartAt, 4, 2) || substr(r.StartAt, 1, 2)";
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
    string sql = "UPDATE Reservations SET TableId=@TableId, GuestCount=@GuestCount WHERE ID=@ID";
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

    public ReservationModel? GetLatestByUserId(int userId)
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

    public bool DoesReservationHaveDishes(ReservationModel reservation)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var query = "SELECT COUNT(1) FROM Reservations_Dishes WHERE ReservationId = @ReservationId";

        int count = connection.ExecuteScalar<int>(query, new { ReservationId = reservation.ID });

        return count > 0;
    }

    public void UpdateReservationGuestCount(ReservationModel reservation)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string query = "UPDATE Reservations SET GuestCount = @GuestCount, UpdatedAt = datetime('now') WHERE Id = @Id";
            connection.Execute(query, new
            {
                GuestCount = reservation.GuestCount,
                Id = reservation.ID
            });
        }
    }

    public void UpdateReservationDateTime(ReservationModel reservation)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string query = "UPDATE Reservations SET StartAt = @Date, UpdatedAt = datetime('now') WHERE Id = @Id";
            connection.Execute(query, new
            {
                Date = reservation.StartAt,
                Id = reservation.ID
            });
        }
    }


    public void UpdateReservationStatus(ReservationModel reservation, string status)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string query = "UPDATE Reservations SET Status = @Status WHERE Id = @Id";
            connection.Execute(query, new
            {
                Status = status,
                Id = reservation.ID
            });
        }
    }

    public string? GetReservationStatus(ReservationModel reservation)
    {
        string sql = @"SELECT Status FROM Reservations WHERE ID = @Id";

        using var connection = new SqliteConnection(_connectionString);

        return connection.QueryFirstOrDefault<string>(sql, new { Id = reservation.ID });
    }

    public ReservationModel? GetReservationByIdSimple(int id)
    {
        string sql = @"SELECT * FROM Reservations WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<ReservationModel>(sql, new { Id = id });
    }
    public List<(string DishName, int Count)> GetDishCountsByDate(string date)
    {
        string sql = @"
            SELECT d.Name as DishName, COUNT(*) as Count
            FROM Reservations_Dishes rd
            JOIN Dishes d ON rd.DishId = d.ID
            JOIN Reservations r ON rd.ReservationId = r.ID
            WHERE substr(r.StartAt, 1, 10) = @Date
            GROUP BY d.Name
            ORDER BY d.Name";
            
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<(string DishName, int Count)>(sql, new { Date = date }).ToList();
    }

    public string? GetGuestAllergies(int reservationId)
    {
        string sql = "SELECT GuestAllergies FROM Reservations WHERE ID = @Id";
        SqliteConnection connection = new SqliteConnection(_connectionString);

        string? allergies =
            connection.QueryFirstOrDefault<string>(sql, new { Id = reservationId });

        return allergies;
    }

    public bool SetGuestAllergies(int reservationId, List<int> allergenIds)
    {
        string sql = "UPDATE Reservations SET GuestAllergies = @Allergies WHERE ID = @Id";
        SqliteConnection connection = new SqliteConnection(_connectionString);

        string allergies = string.Join(",", allergenIds);
        int rowsAffected = connection.Execute(sql, new
        {
            Id = reservationId,
            Allergies = allergies
        });
        return rowsAffected > 0;
    }
}