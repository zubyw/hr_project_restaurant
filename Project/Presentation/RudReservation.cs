using System;
using System.Collections.Generic;
using System.Globalization; // <-- toegevoegd
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
            // needed for ownership checks in logic
            ReservationsLogic.CurrentUserId = userId;

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

            int selectedId;
            if (!int.TryParse(input, out selectedId))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadKey();
                return;
            }

            // must exist in shown list for this user
            bool exists = false;
            for (int i = 0; i < reservations.Count; i++)
            {
                if (reservations[i].ID == selectedId)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                Console.WriteLine("This reservation ID does not exist.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("1. Update reservation");
            Console.WriteLine("2. Cancel reservation");
            Console.WriteLine("3. Back");
            Console.Write("Choose: ");
            string? choice = Console.ReadLine();

            if (choice == "1") Update(selectedId, userId);
            else if (choice == "2") Delete(selectedId);
        }

        // Update a reservation (same step flow as create: guests -> date -> arrow-time)
        private void Update(int id, int userId)
        {
            // final guard: must exist for current user
            if (!logic.ReservationExistsForCurrentUser(id))
            {
                Console.WriteLine("This reservation was not found for your account.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("=== Update Reservation ===");
            Console.WriteLine();

            // 1) New guest count (1–6)
            Console.Write("New guest count (1–6): ");
            string? guestInput = Console.ReadLine();
            int guests;
            if (!int.TryParse(guestInput, out guests) || guests < 1 || guests > 6)
            {
                Console.WriteLine("Given amount of people incorrect (1–6)");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start(userId);
                return;
            }

            // 2) Date (YYYY-MM-DD)
            Console.Clear();
            Console.WriteLine("=== Update Reservation ===");
            Console.WriteLine();
            Console.WriteLine("Date: (YYYY-MM-DD)");
            string? dateIn = Console.ReadLine();

            DateTime dateOnly;
            if (string.IsNullOrEmpty(dateIn) ||
                !DateTime.TryParseExact(dateIn, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateOnly))
            {
                Console.WriteLine("Given date format incorrect (YYYY-MM-DD)");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start(userId);
                return;
            }

            // ⛔ Verleden datum blokkeren
            if (dateOnly.Date < DateTime.Today)
            {
                Console.WriteLine("You cannot select a date in the past.");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start(userId);
                return;
            }

            // 3) Time via arrow-key selector (blue highlight)
            Console.WriteLine("Select Arrival Time:");
            string selectedTime = logic.SelectArrivalTime(); // returns "HH:mm"

            // Parse "HH:mm" naar TimeSpan
            if (!TimeSpan.TryParseExact(selectedTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan timeOfDay))
            {
                Console.WriteLine("Invalid time selected.");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start(userId);
                return;
            }

            // Volledige starttijd
            DateTime startAt = dateOnly.Date + timeOfDay;

            // ⛔ Vandaag + tijd al voorbij blokkeren
            if (startAt < DateTime.Now)
            {
                Console.WriteLine("You cannot select a time in the past.");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start(userId);
                return;
            }

            // Combine to "yyyy-MM-dd HH:mm" (no seconds) en run je bestaande business rules (≥ 17:00, etc.)
            string combined = startAt.ToString("yyyy-MM-dd HH:mm");
            if (!logic.IsValidReservationDateTime(combined))
            {
                Console.WriteLine("Invalid date or time (must be >= 17:00).");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start(userId);
                return;
            }

            // Apply update (logic enforces ownership + 1–6 again)
            logic.UpdateReservationForGuest(id, guests, combined);

            Console.WriteLine("✅ Reservation updated!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Start(userId);
        }

        // Cancel a reservation
        private void Delete(int id)
        {
            // final guard: must exist for current user
            if (!logic.ReservationExistsForCurrentUser(id))
            {
                Console.WriteLine("This reservation was not found for your account.");
                Console.ReadKey();
                return;
            }

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
