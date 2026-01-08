using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace Project.DataAccess
{
    public class DrinkAccess : BaseAccess<Drink>
    {
        protected new  string Table = "Drink";

        public override void Write(Drink drink)
        {
            string sql = $"INSERT INTO {Table} (Name, AlcoholPercentage, Price) VALUES (@Name, @AlcoholPercentage, @Price)";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, drink);
            connection.Close();
        }

        public List<Drink> GetAll()
        {
            string sql = $"SELECT Id, Name, AlcoholPercentage, Price FROM {Table}";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            List<Drink> drinks = connection.Query<Drink>(sql).ToList();
            connection.Close();
            return drinks;
        }

        public Drink GetById(int id)
        {
            string sql = $"SELECT Id, Name, AlcoholPercentage, Price FROM {Table} WHERE Id = @Id";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            Drink drink = connection.QueryFirstOrDefault<Drink>(sql, new { Id = id });
            connection.Close();
            return drink;
        }

        public override void Update(Drink drink)
        {
            string sql = $"UPDATE {Table} SET Name = @Name, AlcoholPercentage = @AlcoholPercentage, Price = @Price WHERE Id = @Id";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, drink);
            connection.Close();
        }

        public bool IsDrinkLinked(int drinkId)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM Dishes
                WHERE DrinkId = @DrinkId";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            int count = connection.ExecuteScalar<int>(sql, new { DrinkId = drinkId });
            connection.Close();

            return count > 0;
        }

        public override void Delete(Drink drink)
        {
            string sql = $"DELETE FROM {Table} WHERE Id = @Id";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(sql, new { Id = drink.ID });
            connection.Close();
        }
    }
}
