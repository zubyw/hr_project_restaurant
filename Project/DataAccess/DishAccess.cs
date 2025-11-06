using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;

namespace Project.DataAccess
{
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
            List<DishModel> AllDishesByType = connection.Query<DishModel>(DishesSql, new { Type = type }).ToList();
            return AllDishesByType;
        }


        public List<int> GetallDishIdByThemeId(int themeid)
        {
            string DishesSql = @"
        SELECT DishId
        FROM Dishes_Themes
        WHERE ThemeId = @ThemeId";

            // SQL Joins

            using var connection = new SqliteConnection(_connectionString);
            List<int> AllDishIdByThemeId = connection.Query<int>(DishesSql, new { ThemeId = themeid }).ToList();
            return AllDishIdByThemeId;
        }

        public List<DishModel> GetDishesByIds(List<int> dishIds)
        {
            string sql = @"
        SELECT ID, Name, Price, Description, Type
        FROM Dishes
        WHERE ID IN @Ids";
            using var connection = new SqliteConnection(_connectionString);
            List<DishModel> AllDishesFromIds = connection.Query<DishModel>(sql, new { Ids = dishIds }).ToList();

            return AllDishesFromIds;
        }

        public int ReservedDishes(DishModel dish, ReservationModel reservation)
        {
            string sql = $"INSERT INTO Reservations_Dishes (ReservationId, DishId) VALUES (@ReservationId, @DishId) RETURNING ID";
            using var connection = new SqliteConnection(_connectionString);

            var parameters = new
            {
                ReservationId = reservation.ID,
                DishId = dish.ID
            };

            int i = connection.QuerySingle<int>(sql, parameters);
            return i;
        }


        public List<DishModel> GetAllDishesByReservation(ReservationModel resm)
        {
            string sql = @"
        SELECT d.ID, d.Name, d.Price, d.Description, d.Type
        FROM Dishes d
        JOIN Reservations_Dishes rd ON d.ID = rd.DishId
        JOIN Reservations r ON r.ID = rd.ReservationId
        WHERE r.ID = @Id";
            using var connection = new SqliteConnection(_connectionString);
            List<DishModel> AllDishesFromIds = connection.Query<DishModel>(sql, new { Id = resm.ID }).ToList();

            return AllDishesFromIds;
        }

        public void DeleteReservationDishes(List<int> ids)
        {
            foreach (int id in ids)
            {
                string sql = $"DELETE FROM Reservations_Dishes WHERE ID = @Id";
                using var connection = new SqliteConnection(_connectionString);
                connection.Execute(sql, new { Id = id });
            }
        }

        public int AddDishReturnId(DishModel dish)
        {
            string sql = "INSERT INTO Dishes (Name, Price, Description, Type) " +
                         "VALUES (@Name, @Price, @Description, @Type); SELECT last_insert_rowid();";
            Microsoft.Data.Sqlite.SqliteConnection connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Open();
            int newId = connection.ExecuteScalar<int>(sql, dish);
            connection.Close();
            return newId;
        }

            public void LinkDishToTheme(int dishId, int themeId)
        {
            string sql = "INSERT INTO Dishes_Themes (DishId, ThemeId) VALUES (@DishId, @ThemeId);";
            Microsoft.Data.Sqlite.SqliteConnection connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dishId, ThemeId = themeId }); // Dapper
            connection.Close();
        }

        public void UnlinkDishFromTheme(int dishId, int themeId)
        {
            string sql = "DELETE FROM Dishes_Themes WHERE DishId = @DishId AND ThemeId = @ThemeId;";
            Microsoft.Data.Sqlite.SqliteConnection connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dishId, ThemeId = themeId }); // Dapper
            connection.Close();
        }   
    }
}