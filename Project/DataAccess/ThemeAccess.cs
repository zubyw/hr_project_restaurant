using Microsoft.Data.Sqlite;
using Dapper;

public class ThemeAccess : BaseAccess<ThemeModel>
{
    protected override string Table { get; } = "Themes";

    public override void Write(ThemeModel theme)
    {
        string sql = $"INSERT INTO {Table} (Name, Course) VALUES (@Name, @Course)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, theme);
    }


    public void UpdateThemeCalendar(int themeId, string type, string description, DateTime timeSlot)
    {
        using var connection = new SqliteConnection(_connectionString);

        string sql = @"
            UPDATE Themes_Calendar
            SET Type = @Type,
                Description = @Description,
                TimeSlot = @TimeSlot
            WHERE ThemeId = @ThemeId;
        ";

        connection.Execute(sql, new
        {
            ThemeId = themeId,
            Type = type,
            Description = description,
            TimeSlot = timeSlot
        });
    }



    public ThemeModel? GetByName(string name)
    {
        string sql = $"SELECT * FROM {Table} WHERE Name = @Name";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<ThemeModel>(sql, new { Name = name });
    }
    public ThemeModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<ThemeModel>(sql, new { Id = id });
    }

    public override void Update(ThemeModel theme)
    {
        string sql = $"UPDATE {Table} SET Name = @Name, Course = @Course WHERE ID = @ID";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, theme);
    }

    public int? GetActiveThemeID()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        DateTime today = DateTime.Today;

        string sql = @"
            SELECT ThemeId 
            FROM Themes_Calendar
            WHERE date(TimeSlot) <= @Today
            ORDER BY date(TimeSlot) DESC
            LIMIT 1;
        ";

        // Keep yyyy-MM-dd format for SQLite date() function compatibility
        int? themeId = connection.ExecuteScalar<int?>(sql, new { Today = today.ToString("yyyy-MM-dd") });

        return themeId;
    }

    public List<ThemeModel> GetAllThemes()
    {
        string sql = $"SELECT * FROM {Table}";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<ThemeModel>(sql).ToList();
    }

    public void DeleteThemeCompletely(ThemeModel theme)
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            string deleteCalendar = "DELETE FROM Themes_Calendar WHERE ThemeId = @Id;";
            connection.Execute(deleteCalendar, new { Id = theme.ID }, transaction);

            string deleteDishLinks = "DELETE FROM Dishes_Themes WHERE ThemeId = @Id;";
            connection.Execute(deleteDishLinks, new { Id = theme.ID }, transaction);

            string deleteTheme = "DELETE FROM Themes WHERE ID = @Id;";
            int rows = connection.Execute(deleteTheme, new { Id = theme.ID }, transaction);
            
            if (rows == 0)
            {
                throw new Exception("Theme not found.");
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ActivateTheme(int themeId)
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string sql = "UPDATE Themes SET IsActive = 1 WHERE ID = @Id;";
        connection.Execute(sql, new { Id = themeId });

        connection.Close();
        Console.WriteLine("Theme activated.");
    }

    public void DeactivateTheme(int themeId)
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string sql = "UPDATE Themes SET IsActive = 0 WHERE ID = @Id;";
        connection.Execute(sql, new { Id = themeId });

        connection.Close();
        Console.WriteLine("Theme deactivated.");
    }
    public Dictionary<string, int> GetFutureThemesByMonth(DateTime fromDate)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Convert fromDate to yyyy-MM-dd for comparison
        string fromDateString = fromDate.ToString("yyyy-MM-dd");
        string fromMonthYear = fromDate.ToString("yyyy-MM");

        string sql = @"
            SELECT TimeSlot, ThemeId
            FROM Themes_Calendar
            WHERE TimeSlot >= @FromDate OR substr(TimeSlot, 1, 7) = @FromMonthYear
            ORDER BY TimeSlot;
        ";

        var rows = connection.Query(sql, new { FromDate = fromDateString, FromMonthYear = fromMonthYear });

        var result = new Dictionary<string, int>();

        foreach (var row in rows)
        {
            string timeSlot = row.TimeSlot; // yyyy-MM-dd
            DateTime date = DateTime.Parse(timeSlot);
            string monthYear = date.ToString("MM-yyyy"); // MM-yyyy string

            int themeId = (int)row.ThemeId;
            result[monthYear] = themeId;
        }

        return result;
    }

    public bool GetThemeByName(string themeName)
        {
            string sql = "SELECT COUNT(*) FROM Themes WHERE Name = @Name";

            using var connection = new SqliteConnection(_connectionString);
            int count = connection.ExecuteScalar<int>(sql, new { Name = themeName });

            return count > 0;
        }
    public List<string> GetThemeCalendarTakenMonths()
    {
        string sql = "SELECT TimeSlot FROM Themes_Calendar";
        using var connection = new SqliteConnection(_connectionString);
        List<string> timeSlots = connection.Query<string>(sql).ToList();
        return timeSlots;
    }

    public void LinkMonthToTheme(string month, ThemeModel theme)
    {
        string sql = $"INSERT INTO Themes_Calendar (ThemeId, TimeSlot) VALUES (@ThemeId, @TimeSlot)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new {ThemeId = theme.ID, TimeSlot = month + "-01"});
    }

    public void DeleteDishonTheme(ThemeModel theme, DishModel dish)
    {
        string sql = $"DELETE FROM Dishes_Themes WHERE DishId = @dishId AND ThemeId = themeId";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { dishId = dish.ID, themeId = theme.ID});
    }

    public ThemeModel? GetCorrectTheme(string date)
    {
        string sql = @"
        SELECT t.*
        FROM Themes t
        INNER JOIN Themes_Calendar tc ON t.ID = tc.ThemeId
        WHERE tc.TimeSlot = @Date
    ";
    using var connection = new SqliteConnection(_connectionString);
    ThemeModel? theme = connection.QueryFirstOrDefault<ThemeModel>(sql, new { Date = date });
    return theme;
    }
}