using System.Globalization;

static class ReservationManagement
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n=== Reservation Management (Admin) ===");
        Console.WriteLine("1. View All Reservations");
        Console.WriteLine("2. View Reservations by Date");
        Console.WriteLine("3. View Reservations by Date Range");
        Console.WriteLine("4. Search Reservations by Guest");
        Console.WriteLine("5. Back to Main Menu");
        Console.Write("Please select an option: ");

        string? input = Console.ReadLine();
        switch (input)
        {
            case "1":
                ViewAllReservations();
                break;
            case "2":
                ViewReservationsByDate();
                break;
            case "3":
                ViewReservationsByDateRange();
                break;
            case "4":
                SearchReservationsByGuest();
                break;
            case "5":
                Menu.ShowMainMenu();
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Start();
                break;
        }
    }

    private static void ViewAllReservations()
    {
        Console.Clear();
        Console.WriteLine("\n=== All Reservations ===");

        var reservations = _reservationsLogic.GetAllReservations();
        
        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations found.");
        }
        else
        {
            DisplayReservationsTable(reservations);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Start();
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

        Console.WriteLine($"\nReservations for {dateToSearch}:");
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

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Start();
    }

    private static void ViewReservationsByDateRange()
    {
        Console.Clear();
        Console.WriteLine("\n=== Reservations by Date Range ===");
        
        Console.Write("Enter start date (YYYY-MM-DD): ");
        string? startDate = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(startDate) || !ReservationsLogic.IsValidDateFormat(startDate))
        {
            Console.WriteLine("Invalid start date format. Please use YYYY-MM-DD format.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            ViewReservationsByDateRange();
            return;
        }

        Console.Write("Enter end date (YYYY-MM-DD): ");
        string? endDate = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(endDate) || !ReservationsLogic.IsValidDateFormat(endDate))
        {
            Console.WriteLine("Invalid end date format. Please use YYYY-MM-DD format.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            ViewReservationsByDateRange();
            return;
        }

        Console.WriteLine($"\nReservations from {startDate} to {endDate}:");
        var reservations = _reservationsLogic.GetReservationsByDateRange(startDate, endDate);
        
        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations found for this date range.");
        }
        else
        {
            // Group by date for better overview
            var groupedReservations = reservations.GroupBy(r => DateTime.Parse(r.StartAt).ToString("yyyy-MM-dd"));
            
            foreach (var group in groupedReservations.OrderBy(g => g.Key))
            {
                Console.WriteLine($"\n--- {group.Key} ---");
                DisplayReservationsTable(group.ToList(), false);
                
                var dayTotal = group.Sum(r => r.GuestCount);
                Console.WriteLine($"Total guests for {group.Key}: {dayTotal}");
            }

            // Overall summary
            var totalGuests = reservations.Sum(r => r.GuestCount);
            var totalConfirmed = reservations.Count(r => r.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase));
            var totalPending = reservations.Count(r => r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
            
            Console.WriteLine($"\n--- Overall Summary ({startDate} to {endDate}) ---");
            Console.WriteLine($"Total Reservations: {reservations.Count}");
            Console.WriteLine($"Confirmed: {totalConfirmed}");
            Console.WriteLine($"Pending: {totalPending}");
            Console.WriteLine($"Total Guests: {totalGuests}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Start();
    }

    private static void SearchReservationsByGuest()
    {
        Console.Clear();
        Console.WriteLine("\n=== Search Reservations by Guest ===");
        Console.Write("Enter guest's email address: ");
        
        string? email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email address cannot be empty.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            SearchReservationsByGuest();
            return;
        }

        // First get the user ID
        var usersLogic = new UsersLogic();
        var user = usersLogic.GetUserByEmail(email);
        
        if (user == null)
        {
            Console.WriteLine($"No user found with email address: {email}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Start();
            return;
        }

        Console.WriteLine($"\nReservations for {user.FirstName} {user.LastName} ({email}):");
        var reservations = _reservationsLogic.GetReservationsByUserId(user.ID);
        
        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations found for this guest.");
        }
        else
        {
            DisplayReservationsTable(reservations);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Start();
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
}
