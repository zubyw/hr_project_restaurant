using System.Globalization;
using Project.DataModels;  

static class ReservationManagement
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n=== Reservation Management (Admin) ===");
        Console.WriteLine("1. View All Reservations");
        Console.WriteLine("2. View Reservations by Date");
        Console.WriteLine("3. Change Reservations");
        Console.WriteLine("4. Back to Main Menu");
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
                Console.WriteLine("Change reservations feature coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Start();
                break;
            case "4":
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
