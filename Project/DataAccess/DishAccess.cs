using Microsoft.Data.Sqlite;
using Dapper;

public class DishAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db";
    private readonly string Table = "Dishes";

    public void AddDish(DishModel dish)
    {
        string sql = $"INSERT INTO {Table} (Name, Price, Description, Type) VALUES (@Name, @Price, @Description, @Type)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, dish);
    }

    public DishModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<DishModel>(sql, new { Id = id });
    }

    public void Update(DishModel dish)
    {
        string sql = $"UPDATE {Table} SET Name = @Name, Price = @Price, Description = @Description, Type = @Type WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, dish);
    }

    public void Delete(DishModel dish)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { Id = dish.ID });
    }

    public List<DishModel> GetDishByType(string type)
    {
        string DishesSql = @"
        SELECT ID, Name, Price, Description, Type
        FROM Dishes
        WHERE Type = @Type";
        using var connection = new SqliteConnection(_connectionString);
        List<DishModel> AllDishesByType = connection.Query<DishModel>(DishesSql, new { Type = type}).ToList();
        return AllDishesByType;
    }
    

}