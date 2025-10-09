using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.DataModels;

namespace Project.DataAccess
{
    public class RudReservationsAccess
    {
        private string connectionString = "Data Source=DataSources/project.db";

        // Method to update a reservation
        public List<ReservationModel> GetReservationsByUserId(int userId)
        {
            List<ReservationModel> list = new List<ReservationModel>();

            // DB connection
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // Query that gets reservations by userId
                string query = "SELECT * FROM Reservations WHERE UserId = @UserId";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ReservationModel res = new ReservationModel();
                        res.Id = reader.GetInt32(0);
                        res.UserId = reader.GetInt32(1);
                        res.TableId = reader.GetInt32(2);
                        res.GuestCount = reader.GetInt32(3);
                        res.StartAt = reader.GetString(4);
                        res.Status = reader.GetString(5);
                        res.CanModifyUntil = reader.GetString(6);
                        res.CreatedAt = reader.GetString(7);
                        res.UpdatedAt = reader.GetString(8);
                        list.Add(res);
                    }
                    reader.Close();
                }
            }
            return list;
        }

        // Method that updates reservation
        public void UpdateReservation(int id, int guestCount, string startAt)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Query that updates reservation by id
                string query = "update Reservations set GuestCount = @GuestCount, StartAt = @StartAt, UpdatedAt = datetime('now') where Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GuestCount", guestCount);
                    command.Parameters.AddWithValue("@StartAt", startAt);
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Method that deletes reservation
        public void DeleteReservation(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // Query that deletes reservation by id
                string query = "DELETE FROM Reservations WHERE ID = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}