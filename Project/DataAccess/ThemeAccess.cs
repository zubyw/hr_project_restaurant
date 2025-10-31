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

}