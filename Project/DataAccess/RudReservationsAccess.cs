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

        
        // updates a reservation by ID 
        public void UpdateReservation(int id, int guestCount, string startAt)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                string query = "UPDATE Reservations SET GuestCount = @GuestCount, StartAt = @StartAt, UpdatedAt = datetime('now') WHERE Id = @Id";
                connection.Execute(query, new
                {
                    GuestCount = guestCount,
                    StartAt = startAt,
                    Id = id
                });
            }
        }

        // deletes a reservation by ID
        public void DeleteReservation(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                string query = "DELETE FROM Reservations WHERE Id = @Id";
                connection.Execute(query, new { Id = id });
            }
        }
    }
}
