using System;
using System.Collections.Generic;
using Project.DataModels;

namespace Project.Presentation
{
    public class RudReservation
    {   
        // Logic layer instance
        ReservationsLogic logic = new ReservationsLogic();

        // Starts the reservation menu
        public void Start(int userId) 
        {
            Console.Clear();
            Console.WriteLine("=== My Reservations ===");
            Console.WriteLine();

            // Load all reservations for the logged-in user
            List<ReservationModel> reservations = logic.GetReservationsByUserIdForGuest(userId);

            // If none, go back
            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            // Table layout (guest view)
            Console.WriteLine();
            Console.WriteLine("┌──────┬────────┬────────┬─────────────────────┬───────────┐");
            Console.WriteLine("│  ID  │ Table  │ Guests │      Date/Time      │  Status   │");
            Console.WriteLine("├──────┼────────┼────────┼─────────────────────┼───────────┤");

            foreach (ReservationModel r in reservations)
            {
                string dateTime = DateTime.Parse(r.StartAt).ToString("MM-dd-yyyy HH:mm");
                Console.WriteLine($"│ {r.ID,4} │ {r.TableId,6} │ {r.GuestCount,6} │ {dateTime,-19} │ {r.Status,-9} │");
            }

            Console.WriteLine("└──────┴────────┴────────┴─────────────────────┴───────────┘");
            Console.WriteLine();

            // Choose reservation to manage
            Console.Write("Enter reservation ID to manage: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("1. Update reservation");
                Console.WriteLine("2. Cancel reservation");
                Console.WriteLine("3. Back");
                Console.Write("Choose: ");
                string? choice = Console.ReadLine();

                if (choice == "1") Update(selectedId);
                else if (choice == "2") Delete(selectedId);
            }
        }

        // Update a reservation
        private void Update(int id)
        {
            Console.Clear();
            Console.WriteLine("=== Update Reservation ===");

            // New guest count
            Console.Write("New guest count: ");
            int guests = Convert.ToInt32(Console.ReadLine());

            // New date/time
            Console.Write("New date/time (YYYY-MM-DD HH:MM): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Try again.");
                Console.ReadKey();
                return;
            }

            // Validate date/time
            bool isValid = logic.IsValidReservationDateTime(input);
            if (!isValid)
            {
                Console.WriteLine("Invalid date or time.");
                Console.ReadKey();
                return;
            }

            // Apply update
            logic.UpdateReservationForGuest(id, guests, input);
            Console.WriteLine("Reservation updated!");
            Console.ReadKey();
        }

        // Cancel a reservation
        private void Delete(int id)
        {
            Console.Clear();
            Console.WriteLine("Cancel this reservation? (y/n)");
            string? answer = Console.ReadLine();

            if (answer != null && answer.ToLower() == "y")
            {
                logic.DeleteReservationForGuest(id);
                Console.WriteLine("Reservation cancelled.");
            }

            Console.ReadKey();
        }
    }
}
