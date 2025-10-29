using Microsoft.Data.Sqlite;
using Dapper;

public class ThemeAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db";
    private readonly string Table = "Themes";

    public void AddTheme(ThemeModel theme)
    {
        string sql = $"INSERT INTO {Table} (Name, Course, IsActive) VALUES (@Name, @Course, @IsActive)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, theme);
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
    


    

}