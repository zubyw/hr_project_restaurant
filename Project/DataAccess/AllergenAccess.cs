using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;

namespace Project.DataAccess
{
    public class AllergenAccess
    {
        private readonly string _connectionString = "Data Source=DataSources/project.db";
        private readonly string Table = "Allergens";

        public List<AllergenModel> GetAll()
        {
            string sql = $"SELECT * FROM {Table} ORDER BY Name";
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<AllergenModel>(sql).ToList();
        }

        public AllergenModel? GetById(int id)
        {
            string sql = $"SELECT * FROM {Table} WHERE ID = @Id";
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<AllergenModel>(sql, new { Id = id });
        }

        public void LinkDishToAllergen(int dishId, int allergenId)
        {
            string sql = "INSERT INTO Dishes_Allergens (DishId, AllergenId) VALUES (@DishId, @AllergenId);";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dishId, AllergenId = allergenId });
        }

        public void UnlinkDishFromAllergen(int dishId, int allergenId)
        {
            string sql = "DELETE FROM Dishes_Allergens WHERE DishId = @DishId AND AllergenId = @AllergenId;";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dishId, AllergenId = allergenId });
        }

        public void UnlinkAllAllergensFromDish(int dishId)
        {
            string sql = "DELETE FROM Dishes_Allergens WHERE DishId = @DishId;";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dishId });
        }

        public List<int> GetAllergenIdsByDishId(int dishId)
        {
            string sql = "SELECT AllergenId FROM Dishes_Allergens WHERE DishId = @DishId";
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<int>(sql, new { DishId = dishId }).ToList();
        }

        public List<AllergenModel> GetAllergensByDishId(int dishId)
        {
            string sql = @"
                SELECT a.ID, a.Name, a.Description
                FROM Allergens a
                JOIN Dishes_Allergens da ON da.AllergenId = a.ID
                WHERE da.DishId = @DishId
                ORDER BY a.Name";
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<AllergenModel>(sql, new { DishId = dishId }).ToList();
        }
    }
}
