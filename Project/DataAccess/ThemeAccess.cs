using Microsoft.Data.Sqlite;
using Dapper;

public class ThemeAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db";
    private readonly string Table = "Themes";

    public void AddTheme(ThemeModel theme, DateTime TimeSlot)
    {
        string sql = $"INSERT INTO {Table} (Name, Course, IsActive) VALUES (@Name, @Course, @IsActive)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, theme);

        int themeId = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");

        string insertCalendarSql = @"
        INSERT INTO Themes_Calendar (ThemeId, Type, TimeSlot, Description)
        VALUES (@ThemeID, @Type, @TimeSlot, @Description);
    ";

    var calendarParams = new
    {
        ThemeID = themeId,
        Type = theme.Name,
        TimeSlot,
        Description = theme.Course
    };

    connection.Execute(insertCalendarSql, calendarParams);

    }


    public ThemeModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<ThemeModel>(sql, new { Id = id });
    }

    public void Update(ThemeModel theme)
    {
        string sql = $"UPDATE {Table} SET Name = @Name, Course = @Course, IsActive = @IsActive WHERE ID = @ID";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, theme);
    }

    public void Delete(ThemeModel theme)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { Id = theme.ID });
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

        int? themeId = connection.ExecuteScalar<int?>(sql, new { Today = today.ToString("yyyy-MM-dd") });

        return themeId;
    }

    public List<ThemeModel> GetAllThemes()
    {
        string sql = $"SELECT * FROM {Table}";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<ThemeModel>(sql).ToList();
    }

    public void DeleteThemeCompletely(int themeId)
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            string deleteCalendar = "DELETE FROM Themes_Calendar WHERE ThemeId = @Id;";
            connection.Execute(deleteCalendar, new { Id = themeId }, transaction);

            string deleteDishLinks = "DELETE FROM Dishes_Themes WHERE ThemeId = @Id;";
            connection.Execute(deleteDishLinks, new { Id = themeId }, transaction);

            string deleteTheme = "DELETE FROM Themes WHERE ID = @Id;";
            int rows = connection.Execute(deleteTheme, new { Id = themeId }, transaction);
            
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
}