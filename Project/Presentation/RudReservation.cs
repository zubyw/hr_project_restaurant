using System;
using System.Collections.Generic;
using Project.Logic;
using Project.DataModels;

namespace Project.Presentation
{
    public class RudReservation
    {   
        RudReservationsLogic logic = new RudReservationsLogic();

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

            foreach (ReservationModel r in reservations)
            {
                Console.WriteLine($"ID: {r.Id} | Table: {r.TableId} | Guests: {r.GuestCount} | Date: {r.StartAt} | Status: {r.Status}");
            }

            Console.WriteLine();
            Console.Write("Enter reservation ID to manage: ");
            string input = Console.ReadLine();

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
    }
}

