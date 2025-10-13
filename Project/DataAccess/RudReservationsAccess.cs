using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;

namespace Project.DataAccess
{
    public class RudReservationsAccess
    {
        private string connectionString = "Data Source=DataSources/project.db";

        // Get a reservation by ID
        public List<ReservationModel> GetReservationsByUserId(int userId)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                string query = "SELECT * FROM Reservations WHERE UserId = @UserId";
                List<ReservationModel> list = connection.Query<ReservationModel>(query, new { UserId = userId }).AsList();
                return list;
            }
        }
    }
}
