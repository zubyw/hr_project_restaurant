using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace Project.DataAccess
{
    public class DrinkAccess
    {
        private readonly string _connectionString = "Data Source=DataSources/project.db";
        private readonly string Table = "Drink";

        public void Write(Drink drink)
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
    }
}
