using System.Globalization;
using System.Linq;
using Project.Logic;
using Project.DataModels;
using Project.Presentation;

static class ReservationManagement
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
    public static void Start()
    {
        string[] options = new string[] { "View All Reservations", "View Reservations by Date", "Back to Main Menu" };
        int selectedIndex = 0;

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine("\n=== Reservation Management (Admin) ===");
            
            // Display options
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"  {options[i]}");
                Console.ResetColor();
            }

            key = Console.ReadKey(true).Key;

            // Handle arrow keys
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    break;
                case ConsoleKey.Enter:
                    switch (selectedIndex)
                    {
                        case 0:
                            ViewAllReservationsWithOptions();
                            break;
                        case 1:
                            ViewReservationsByDate();
                            break;
                        case 2:
                            Menu.ShowMainMenu();
                            break;
                    }
                    break;
            }
        } while (key != ConsoleKey.Enter);
    }

    private static void ViewAllReservationsWithOptions()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n=== All Reservations ===");

            var reservations = _reservationsLogic.GetAllReservations();
            
            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                Start();
                return;
            }

            DisplayReservationsTable(reservations);

            Console.WriteLine("\nOptions:");
            
            string[] options = new string[] { "Change reservation time", "Change number of guests", "Cancel reservation", "Back to main menu" };
            int selectedIndex = 0;

            ConsoleKey key;
            bool choosingOption = true;
            while (choosingOption)
            {
                // Display options
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine($"  {options[i]}");
                    Console.ResetColor();
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                        Console.Clear();
                        Console.WriteLine("\n=== All Reservations ===");
                        DisplayReservationsTable(reservations);
                        Console.WriteLine("\nOptions:");
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Length;
                        Console.Clear();
                        Console.WriteLine("\n=== All Reservations ===");
                        DisplayReservationsTable(reservations);
                        Console.WriteLine("\nOptions:");
                        break;
                    case ConsoleKey.Enter:
                        choosingOption = false;
                        switch (selectedIndex)
                        {
                            case 0:
                                ChangeReservationTime(reservations);
                                break;
                            case 1:
                                ChangeGuestCount(reservations);
                                break;
                            case 2:
                                CancelReservation(reservations);
                                break;
                            case 3:
                                Start();
                                return;
                        }
                        break;
                }
            }
        }
    }

    private static void ViewReservationsByDate()
    {
        Console.Clear();
        Console.WriteLine("\n=== Reservations by Date ===");
        Console.Write("Enter date (YYYY-MM-DD) or press Enter for today: ");
        
        string? input = Console.ReadLine();
        string dateToSearch;

        if (string.IsNullOrWhiteSpace(input))
        {
            dateToSearch = DateTime.Today.ToString("yyyy-MM-dd");
        }
        else
        {
            if (!ReservationsLogic.IsValidDateFormat(input))
            {
                Console.WriteLine("Invalid date format. Please use YYYY-MM-DD format.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ViewReservationsByDate();
                return;
            }
            dateToSearch = input;
        }

        string[] options = new string[] { "View Another Date", "Back to Main Menu" };
        int selectedIndex = 0;

        do
        {
            Console.Clear();
            Console.WriteLine($"\n=== Reservations for {dateToSearch} ===");
            var reservations = _reservationsLogic.GetReservationsByDate(dateToSearch);
            
            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found for this date.");
            }
            else
            {
                DisplayReservationsTable(reservations);
                
                // Display summary
                var totalGuests = reservations.Sum(r => r.GuestCount);
                var confirmedReservations = reservations.Count(r => r.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase));
                var pendingReservations = reservations.Count(r => r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
                
                Console.WriteLine($"\n--- Summary for {dateToSearch} ---");
                Console.WriteLine($"Total Reservations: {reservations.Count}");
                Console.WriteLine($"Confirmed: {confirmedReservations}");
                Console.WriteLine($"Pending: {pendingReservations}");
                Console.WriteLine($"Total Guests: {totalGuests}");
            }

            Console.WriteLine("\nOptions:");
            // Display options
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"  {options[i]}");
                Console.ResetColor();
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    break;
                case ConsoleKey.Enter:
                    switch (selectedIndex)
                    {
                        case 0:
                            ViewReservationsByDate();
                            return;
                        case 1:
                            Start();
                            return;
                    }
                    break;
            }
        } while (true);
    }

    private static void DisplayReservationsTable(List<ReservationModel> reservations, bool showHeader = true)
    {
        if (showHeader)
        {
            Console.WriteLine();
            Console.WriteLine("┌──────┬─────────┬──────────────────────────────┬───────┬─────────────────────┬───────────┐");
            Console.WriteLine("│  ID  │  Table  │          Guest Name          │ Count │      Date/Time      │  Status   │");
            Console.WriteLine("├──────┼─────────┼──────────────────────────────┼───────┼─────────────────────┼───────────┤");
        }

        foreach (var reservation in reservations)
        {
            var guestName = $"{reservation.GuestFirstName} {reservation.GuestLastName}";
            var dateTime = DateTime.Parse(reservation.StartAt).ToString("MM/dd/yyyy HH:mm");
            
            // Truncate guest name if too long
            if (guestName.Length > 28)
            {
                guestName = guestName.Substring(0, 25) + "...";
            }
            
            Console.WriteLine($"│ {reservation.ID,4} │ {reservation.TableNumber,2} ({reservation.TableCapacity})  │ {guestName,-28} │  {reservation.GuestCount,2}   │ {dateTime,-19} │ {reservation.Status,-9} │");
        }

        if (showHeader)
        {
            Console.WriteLine("└──────┴─────────┴──────────────────────────────┴───────┴─────────────────────┴───────────┘");
        }
    }

    private static void ChangeReservationTime(List<ReservationModel> reservations)
    {
        Console.Write("\nEnter reservation ID to change: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var reservation = reservations.FirstOrDefault(r => r.ID == reservationId);
        if (reservation == null)
        {
            Console.WriteLine("Reservation not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nCurrent reservation: {reservation.GuestFirstName} {reservation.GuestLastName}");
        Console.WriteLine($"Current date/time: {DateTime.Parse(reservation.StartAt):yyyy-MM-dd HH:mm}");
        Console.Write("Enter new date/time (YYYY-MM-DD HH:mm): ");
        
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("No input provided. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newTime))
        {
            Console.WriteLine("Invalid date format. Please use YYYY-MM-DD HH:mm format. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (newTime <= DateTime.Now)
        {
            Console.WriteLine("Cannot set reservation time in the past. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationTime(reservationId, newTime);
        if (success)
        {
            Console.WriteLine("✓ Reservation time updated successfully!");
        }
        else
        {
            Console.WriteLine("✗ Failed to update reservation time.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void ChangeGuestCount(List<ReservationModel> reservations)
    {
        Console.Write("\nEnter reservation ID to change: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var reservation = reservations.FirstOrDefault(r => r.ID == reservationId);
        if (reservation == null)
        {
            Console.WriteLine("Reservation not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nCurrent reservation: {reservation.GuestFirstName} {reservation.GuestLastName}");
        Console.WriteLine($"Current guest count: {reservation.GuestCount}");
        Console.Write("Enter new number of guests (2, 4, or 6): ");
        
        if (!int.TryParse(Console.ReadLine(), out int newCount))
        {
            Console.WriteLine("Invalid number. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (newCount != 2 && newCount != 4 && newCount != 6)
        {
            Console.WriteLine("Guest count must be 2, 4, or 6. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationPersons(reservationId, newCount);
        if (success)
        {
            Console.WriteLine("✓ Guest count updated successfully!");
        }
        else
        {
            Console.WriteLine("✗ Failed to update guest count.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void CancelReservation(List<ReservationModel> reservations)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n=== All Reservations ===");
            DisplayReservationsTable(reservations);
            
            Console.Write("\nEnter reservation ID to cancel (or press Enter to go back): ");
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            if (!int.TryParse(input, out int reservationId))
            {
                Console.WriteLine("Invalid ID. Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            var reservation = reservations.FirstOrDefault(r => r.ID == reservationId);
            if (reservation == null)
            {
                Console.WriteLine("Reservation not found. Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            Console.WriteLine($"\nReservation to cancel: {reservation.GuestFirstName} {reservation.GuestLastName}");
            Console.WriteLine($"Date/time: {DateTime.Parse(reservation.StartAt):yyyy-MM-dd HH:mm}");
            Console.Write("Are you sure you want to cancel this reservation? (y/n): ");
            
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("Cancellation aborted.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            bool success = _reservationsLogic.CancelReservation(reservationId);
            if (success)
            {
                Console.WriteLine("✓ Reservation cancelled successfully!");
                // Refresh the reservations list
                reservations = _reservationsLogic.GetAllReservations();
            }
            else
            {
                Console.WriteLine("✗ Failed to cancel reservation.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
    }
}
