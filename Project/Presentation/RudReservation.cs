using System;
using System.Collections.Generic;
using Project.Logic;
using Project.DataModels;

namespace Project.Presentation
{
    public class RudReservation
    {   
        RudReservationsLogic logic = new RudReservationsLogic();

        // In this method it starts the reservation menu
        public void Start(int userId) 
        {
            Console.Clear(); // Clears the console before showing reservationMenu 
            Console.WriteLine("=== My Reservations ===");
            Console.WriteLine();

            // All reservations will be loaded that belongs to the loggin in user
            List<ReservationModel> reservations = logic.GetReservations(userId);

            // If no reservations are found, return to previous menu
            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            // Display all reservations with the specific id
            foreach (ReservationModel r in reservations)
            {
                Console.WriteLine($"ID: {r.ID} | Table: {r.TableId} | Guests: {r.GuestCount} | Date: {r.StartAt} | Status: {r.Status}");
            }

            Console.WriteLine();
            Console.Write("Enter reservation ID to manage: ");
            string input = Console.ReadLine();

            // UserInput to update or delete
            if (int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("1. Update reservation");
                Console.WriteLine("2. Cancel reservation");
                Console.WriteLine("3. Back");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Update(selectedId);
                }
                else if (choice == "2")
                {
                    Delete(selectedId);
                }
            }            
        }

        // Method that update the reservation
        private void Update(int id)
        {
            Console.Clear();
            Console.WriteLine("=== Update Reservation ===");
            Console.Write("New guest count: ");
            int guests = Convert.ToInt32(Console.ReadLine());

            Console.Write("New date/time (YYYY-MM-DD HH:MM): ");
            string startAt = Console.ReadLine();
            
            // The logic will be called to update the reservation
            logic.UpdateReservation(id, guests, startAt);
            Console.WriteLine("Reservation updated!");
            Console.ReadKey();
        }

        // Method that delete the reservation
        private void Delete(int id)
        {
            Console.Clear();
            Console.WriteLine("Cancel this reservation? (y/n)");
            string answer = Console.ReadLine();

            // If answer is yes, logic will delete the reservation
            if (answer.ToLower() == "y")
            {
                logic.DeleteReservation(id);
                Console.WriteLine("Reservation cancelled.");
            }

            Console.ReadKey();
        }
    }
}