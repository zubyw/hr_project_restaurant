using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;

namespace Project.DataAccess
{
    public class DishAccess : BaseAccess<DishModel>
    {
        protected override string Table { get; } = "Dishes";
        private readonly AllergenAccess _allergenAccess = new AllergenAccess();

        public override void Write(DishModel dish)
        {
            string sql = $"INSERT INTO {Table} (Name, Price, Description, Type) VALUES (@Name, @Price, @Description, @Type)";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, dish);
        }

        public DishModel? GetById(int id)
        {
            string sql = $"SELECT * FROM {Table} WHERE ID = @Id";
            using var connection = new SqliteConnection(_connectionString);
            var dish = connection.QueryFirstOrDefault<DishModel>(sql, new { Id = id });
            
            if (dish != null)
            {
                PopulateAllergenInfo(dish);
            }
            
            return dish;
        }

        private void PopulateAllergenInfo(DishModel dish)
        {
            var allergens = _allergenAccess.GetAllergensByDishId(dish.ID);
            dish.AllergenIds = allergens.Select(a => a.ID).ToList();
            dish.AllergenNames = allergens.Select(a => a.Name).ToList();
        }

        private void PopulateAllergenInfo(List<DishModel> dishes)
        {
            foreach (var dish in dishes)
            {
                PopulateAllergenInfo(dish);
            }
        }

        public override void Update(DishModel dish)
        {
            string sql = $"UPDATE {Table} SET Name = @Name, Price = @Price, Description = @Description, Type = @Type WHERE ID = @ID";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, dish);
        }

        public List<DishModel> GetDishByType(string type)
        {
            string DishesSql = @"
        SELECT ID, Name, Price, Description, Type
        FROM Dishes
        WHERE Type = @Type";
            using var connection = new SqliteConnection(_connectionString);
            List<DishModel> AllDishesByType = connection.Query<DishModel>(DishesSql, new { Type = type }).ToList();
            PopulateAllergenInfo(AllDishesByType);
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
            PopulateAllergenInfo(AllDishesFromIds);
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
            PopulateAllergenInfo(AllDishesFromIds);
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

        public void DeleteDishesOnReservation(ReservationModel reservation)
        {
            string sql = $"DELETE FROM Reservations_Dishes WHERE ReservationId = @Id";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { Id = reservation.ID });
        }

        public int AddDishReturnId(DishModel dish)
        {          
            const string sql = @"
                INSERT INTO Dishes (Name, Price, Description, Type)
                VALUES (@Name, @Price, @Description, @Type);
                SELECT last_insert_rowid();";

            using var connection = new SqliteConnection(_connectionString);
            int newId = connection.QuerySingle<int>(sql, dish);
            return newId;
        }
        

        public void LinkDishToTheme(DishModel dish, ThemeModel theme)
        {
            string sql = "INSERT INTO Dishes_Themes (DishId, ThemeId) VALUES (@DishId, @ThemeId);";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dish.ID, ThemeId = theme.ID });
            connection.Close();
        }

        public void UnlinkDishFromTheme(int dishId, int themeId)
        {
            string sql = "DELETE FROM Dishes_Themes WHERE DishId = @DishId AND ThemeId = @ThemeId;";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { DishId = dishId, ThemeId = themeId });
            connection.Close();
        }

        public bool ExistsByNameTypeInTheme(int themeId, string name, string type)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM Dishes d
                JOIN Dishes_Themes dt ON dt.DishId = d.ID
                WHERE dt.ThemeId = @ThemeId
                AND LOWER(d.Name) = LOWER(@Name)
                AND LOWER(d.Type) = LOWER(@Type);";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            int count = connection.ExecuteScalar<int>(sql, new { ThemeId = themeId, Name = name, Type = type });
            connection.Close();
            return count > 0;
        }

        public List<DishModel> GetByTheme(ThemeModel theme)
        {
            string sql = @"
                SELECT d.ID, d.Name, d.Price, d.Description, d.Type
                FROM Dishes d
                JOIN Dishes_Themes dt ON dt.DishId = d.ID
                WHERE dt.ThemeId = @ThemeId
                ORDER BY d.Type, d.Name;";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            List<DishModel> list = connection.Query<DishModel>(sql, new { ThemeId = theme.ID }).ToList();
            connection.Close();
            PopulateAllergenInfo(list);
            return list;
        }

        public bool GetDishByName(string dishName)
        {
            string sql = "SELECT COUNT(*) FROM Dishes WHERE Name = @Name";

            using var connection = new SqliteConnection(_connectionString);
            int count = connection.ExecuteScalar<int>(sql, new { Name = dishName });

            return count > 0;
        }

        public List<DishModel> GetAllDishes()
        {
            string sql = @"
            SELECT ID, Name, Price, Description, Type
            FROM Dishes";
            using var connection = new SqliteConnection(_connectionString);
            List<DishModel> AllDishes = connection.Query<DishModel>(sql).ToList();
            PopulateAllergenInfo(AllDishes);
            return AllDishes;
        }

        public void DeleteDishes_Themes(DishModel dish)
        {
            string sql = $"DELETE FROM Dishes_Themes WHERE DishId = @Id";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { Id = dish.ID });
        }

        public void DeleteReservationDishes(DishModel dish)
        {
            string sql = $"DELETE FROM Reservations_Dishes WHERE DishId = @Id";
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { Id = dish.ID });
        }


        public List<DishModel> GetMainDishesWithoutDrink()
        {
            string sql = @"
                SELECT ID, Name, Price, Description, Type
                FROM Dishes
                WHERE Type = 'Main'
                AND DrinkId IS NULL";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            List<DishModel> dishes = connection.Query<DishModel>(sql).ToList();
            connection.Close();

            PopulateAllergenInfo(dishes);
            return dishes;
        }

        public void LinkDrinkToDish(int dishId, int drinkId)
        {
            string sql = @"
                UPDATE Dishes
                SET DrinkId = @DrinkId
                WHERE ID = @DishId
                AND Type = 'Main'";

            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { DrinkId = drinkId, DishId = dishId });
        }
        public List<DishModel> GetAllDishesWithATheme()
        {
            string sql = @"
            SELECT DISTINCT d.ID, d.Name, d.Price, d.Description, d.Type
            FROM Dishes d
            JOIN Dishes_Themes dt ON dt.DishId = d.ID
            ORDER BY d.Type, d.Name;";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            List<DishModel> list = connection.Query<DishModel>(sql).ToList();
            connection.Close();
            return list;
        }

        public List<ThemeModel> getAllThemesLinkedToDish(DishModel dish)
        {
            string sql = @"
            SELECT t.ID AS ID, t.Name AS Name, t.Course AS Course
            FROM Themes t
            JOIN Dishes_Themes dt ON dt.ThemeId = t.ID
            WHERE dt.DishId = @DishId;";

            using (var connection = new SqliteConnection(_connectionString))
            {
                List<ThemeModel> list = connection.Query<ThemeModel>(sql, new { DishId = dish.ID }).ToList();
                return list;
            }
        }


    }
}
