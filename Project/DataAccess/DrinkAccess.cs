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
    }
}
