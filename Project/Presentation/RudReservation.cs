using System;
using System.Collections.Generic;
using Project.DataModels;

namespace Project.Presentation
{
    public class RudReservation
    {   
        private readonly ReservationsLogic logic = new ReservationsLogic();

        // Starts the reservation menu
        public void Start(int userId) 
        {
            Console.Clear(); 
            Console.WriteLine("=== My Reservations ===");
            Console.WriteLine();

            List<ReservationModel> reservations = logic.GetReservationsByUserIdForGuest(userId);

            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            // === Table layout ===
            Console.WriteLine();
            Console.WriteLine("┌──────┬─────────┬───────────┬─────────────────────┬───────────┐");
            Console.WriteLine("│  ID  │  Table  │  Guests   │      Date/Time      │  Status   │");
            Console.WriteLine("├──────┼─────────┼───────────┼─────────────────────┼───────────┤");

            foreach (ReservationModel r in reservations)
            {
                string dateTime = DateTime.Parse(r.StartAt).ToString("MM/dd/yyyy HH:mm");
                Console.WriteLine(
                    $"│ {r.ID,4} │ {r.TableId,7} │ {r.GuestCount,7} │ {dateTime,-19} │ {r.Status,-9} │"
                );
            }

            Console.WriteLine("└──────┴─────────┴───────────┴─────────────────────┴───────────┘");
            Console.WriteLine();

            Console.Write("Enter reservation ID to manage: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("1. Update reservation");
                Console.WriteLine("2. Cancel reservation");
                Console.WriteLine("3. Back");
                Console.Write("Choose: ");
                string? choice = Console.ReadLine();

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

        // Updates an existing reservation
        private void Update(int id)
        {
            Console.Clear();
            Console.WriteLine("=== Update Reservation ===");
            Console.Write("New guest count: ");
            int guests = Convert.ToInt32(Console.ReadLine());

            Console.Write("New date/time (YYYY-MM-DD HH:MM): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Try again.");
                Console.ReadKey();
                return;
            }

            bool isValid = logic.IsValidReservationDateTime(input);

            if (!isValid)
            {
                Console.WriteLine("Invalid date or time.");
                Console.ReadKey();
                return;
            }

            logic.UpdateReservationForGuest(id, guests, input);
            Console.WriteLine("Reservation updated!");
            Console.ReadKey();
        }

        // Deletes (cancels) a reservation
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
