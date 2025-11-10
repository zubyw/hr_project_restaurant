using System.Globalization;
using System.Linq;
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
        var reservations = _reservationsLogic.GetAllReservations()
            .OrderBy(r => DateTime.Parse(r.StartAt))
            .ToList();
        
        if (reservations.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("\n=== All Reservations ===");
            Console.WriteLine("No reservations found.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            Start();
            return;
        }

        int selectedReservationIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("\n=== All Reservations ===");

            DisplayReservationsTableWithSelection(reservations, selectedReservationIndex);

            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedReservationIndex = (selectedReservationIndex - 1 + reservations.Count) % reservations.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedReservationIndex = (selectedReservationIndex + 1) % reservations.Count;
                    break;
                case ConsoleKey.Enter:
                    EditReservationFields(reservations[selectedReservationIndex]);
                    reservations = _reservationsLogic.GetAllReservations()
                        .OrderBy(r => DateTime.Parse(r.StartAt))
                        .ToList();
                    if (reservations.Count == 0)
                    {
                        Start();
                        return;
                    }
                    if (selectedReservationIndex >= reservations.Count)
                    {
                        selectedReservationIndex = reservations.Count - 1;
                    }
                    break;
                case ConsoleKey.Escape:
                    Start();
                    return;
            }
        } while (true);
    }

    private static void DisplayReservationsTableWithSelection(List<ReservationModel> reservations, int selectedIndex)
    {
        Console.WriteLine("┌──────┬─────────┬──────────────────────────────┬───────┬─────────────────────┬───────────┐");
        Console.WriteLine("│  ID  │  Table  │          Guest Name          │ Count │      Date/Time      │  Status   │");
        Console.WriteLine("├──────┼─────────┼──────────────────────────────┼───────┼─────────────────────┼───────────┤");

        for (int i = 0; i < reservations.Count; i++)
        {
            var reservation = reservations[i];
            var guestName = $"{reservation.GuestFirstName} {reservation.GuestLastName}";
            var dateTime = DateTime.Parse(reservation.StartAt).ToString("dd-MM-yyyy HH:mm");
            var status = reservation.Status;
            
            if (guestName.Length > 28)
            {
                guestName = guestName.Substring(0, 25) + "...";
            }

            if (status.Length > 9)
            {
                status = status.Substring(0, 9);
            }

            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine($"│ {reservation.ID,4} │ {reservation.TableNumber,2} ({reservation.TableCapacity})  │ {guestName,-28} │  {reservation.GuestCount,2}   │ {dateTime,-19} │ {status,-9} │");
            Console.ResetColor();
        }

        Console.WriteLine("└──────┴─────────┴──────────────────────────────┴───────┴─────────────────────┴───────────┘");
    }

    private static void EditReservationFields(ReservationModel reservation)
    {
        string[] fields = new string[] { "Date/Time", "Guest Count", "Cancel Reservation", "Back" };
        int selectedField = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("\n=== Edit Reservation ===\n");
            Console.WriteLine($"Guest: {reservation.GuestFirstName} {reservation.GuestLastName}");
            Console.WriteLine($"Email: {reservation.GuestEmail}");
            Console.WriteLine($"Table: {reservation.TableNumber} ({reservation.TableCapacity} seats)\n");

            for (int i = 0; i < fields.Length; i++)
            {
                if (i == selectedField)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                switch (i)
                {
                    case 0:
                        Console.WriteLine($"  Date/Time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}");
                        break;
                    case 1:
                        Console.WriteLine($"  Guest Count: {reservation.GuestCount}");
                        break;
                    case 2:
                        Console.WriteLine($"  Cancel Reservation");
                        break;
                    case 3:
                        Console.WriteLine($"  Back");
                        break;
                }
                Console.ResetColor();
            }

            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedField = (selectedField - 1 + fields.Length) % fields.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedField = (selectedField + 1) % fields.Length;
                    break;
                case ConsoleKey.Enter:
                    switch (selectedField)
                    {
                        case 0:
                            ChangeReservationTimeForSingle(reservation);
                            // Refresh reservation data
                            var updated = _reservationsLogic.GetReservationById(reservation.ID);
                            if (updated != null) reservation = updated;
                            break;
                        case 1:
                            ChangeGuestCountForSingle(reservation);
                            updated = _reservationsLogic.GetReservationById(reservation.ID);
                            if (updated != null) reservation = updated;
                            break;
                        case 2:
                            CancelSingleReservation(reservation);
                            return;
                        case 3:
                            return;
                    }
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        } while (true);
    }

    private static void ViewReservationsByDate()
    {
        Console.Clear();
        Console.WriteLine("\n=== Reservations by Date ===");
        Console.Write("Enter date (DD-MM-YYYY) or press Enter for today: ");
        
        string? input = Console.ReadLine();
        string dateToSearch;

        if (string.IsNullOrWhiteSpace(input))
        {
            dateToSearch = DateTime.Today.ToString("dd-MM-yyyy");
        }
        else
        {
            if (!ReservationsLogic.IsValidDateFormat(input))
            {
                Console.WriteLine("Invalid date format. Please use DD-MM-YYYY format.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ViewReservationsByDate();
                return;
            }
            dateToSearch = input;
        }

        var reservations = _reservationsLogic.GetReservationsByDate(dateToSearch);

        if (reservations.Count == 0)
        {
            string[] emptyOptions = new string[] { "View Another Date", "Back to Main Menu" };
            int selectedIndex = 0;

            // Get floor plan data
            TableAcces tableAccess = new TableAcces();
            List<TableModel> allTables = tableAccess.GetAllTables();
            List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(dateToSearch, 0);

            do
            {
                Console.Clear();
                Console.WriteLine($"\n=== Reservations for {dateToSearch} ===");
                Console.WriteLine("\nNo reservations found for this date.");
                Console.WriteLine("\nUse ↑↓ to select, ENTER to confirm, ESC to go back\n");

                for (int i = 0; i < emptyOptions.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine($"  {emptyOptions[i]}");
                    Console.ResetColor();
                }

                // Display floor plan
                DisplayFloorPlanLegend();
                Console.WriteLine();
                DisplayAdminFloorPlan(allTables, reservedTableIds);

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + emptyOptions.Length) % emptyOptions.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % emptyOptions.Length;
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
                    case ConsoleKey.Escape:
                        Start();
                        return;
                }
            } while (true);
        }

        // Show reservations with options
        ShowReservationsForDate(dateToSearch, reservations);
    }

    private static void ShowReservationsForDate(string dateToSearch, List<ReservationModel> reservations)
    {
        string[] options = new string[] { "Edit Reservation", "View Another Date", "Back to Main Menu" };
        int selectedIndex = 0;
        int selectedReservationIndex = 0;
        bool inReservationList = true;

        // Get floor plan data
        TableAcces tableAccess = new TableAcces();
        List<TableModel> allTables = tableAccess.GetAllTables();
        List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(dateToSearch, 0);

        do
        {
            Console.Clear();
            Console.WriteLine($"\n=== Reservations for {dateToSearch} ===");
            
            if (inReservationList)
            {
                DisplayReservationsTableWithSelection(reservations, selectedReservationIndex);
            }
            else
            {
                Console.WriteLine();
                DisplayReservationsTable(reservations);
                Console.WriteLine("\nUse ↑↓ to select option, ENTER to confirm, TAB for reservations, ESC to go back\n");
                
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
            }
            
            // Display floor plan
            DisplayFloorPlanLegend();
            Console.WriteLine();
            DisplayAdminFloorPlan(allTables, reservedTableIds);

            ConsoleKey key = Console.ReadKey(true).Key;

            if (inReservationList)
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedReservationIndex = (selectedReservationIndex - 1 + reservations.Count) % reservations.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedReservationIndex = (selectedReservationIndex + 1) % reservations.Count;
                        break;
                    case ConsoleKey.Enter:
                        EditReservationFields(reservations[selectedReservationIndex]);
                        reservations = _reservationsLogic.GetReservationsByDate(dateToSearch)
                            .OrderBy(r => DateTime.Parse(r.StartAt))
                            .ToList();
                        // Refresh floor plan data after editing
                        reservedTableIds = tableAccess.GetNonAvailableOnDate(dateToSearch, 0);
                        if (reservations.Count == 0)
                        {
                            ViewReservationsByDate();
                            return;
                        }
                        if (selectedReservationIndex >= reservations.Count)
                        {
                            selectedReservationIndex = reservations.Count - 1;
                        }
                        break;
                    case ConsoleKey.Tab:
                        inReservationList = false;
                        selectedIndex = 0;
                        break;
                    case ConsoleKey.Escape:
                        Start();
                        return;
                }
            }
            else
            {
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
                                inReservationList = true;
                                selectedReservationIndex = 0;
                                break;
                            case 1:
                                ViewReservationsByDate();
                                return;
                            case 2:
                                Start();
                                return;
                        }
                        break;
                    case ConsoleKey.Tab:
                        inReservationList = true;
                        selectedReservationIndex = 0;
                        break;
                    case ConsoleKey.Escape:
                        Start();
                        return;
                }
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
            var dateTime = DateTime.Parse(reservation.StartAt).ToString("dd-MM-yyyy HH:mm");
            var status = reservation.Status;
            
            // Truncate guest name if too long
            if (guestName.Length > 28)
            {
                guestName = guestName.Substring(0, 25) + "...";
            }

            // Truncate status if too long
            if (status.Length > 9)
            {
                status = status.Substring(0, 9);
            }
            
            Console.WriteLine($"│ {reservation.ID,4} │ {reservation.TableNumber,2} ({reservation.TableCapacity})  │ {guestName,-28} │  {reservation.GuestCount,2}   │ {dateTime,-19} │ {status,-9} │");
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
        Console.WriteLine($"Current date/time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}");
        Console.Write("Enter new date/time (DD-MM-YYYY HH:mm): ");

        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("No input provided. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (!DateTime.TryParseExact(input, "dd-MM-yyyy", null, DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date format. Please use YYYY-MM-DD HH:mm format. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (!DateTime.TryParseExact(input, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out DateTime newTime))
        {
            Console.WriteLine("Invalid date format. Please use DD-MM-YYYY HH:mm format. Press any key to continue...");
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
            Console.WriteLine("Reservation time updated successfully!");
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
        Console.Write("Enter new number of guests (1-6): ");
        
        if (!int.TryParse(Console.ReadLine(), out int newCount))
        {
            Console.WriteLine("Invalid number. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (newCount < 1 || newCount > 6)
        {
            Console.WriteLine("Guest count must be between 1 and 6. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        // Round up to nearest table size: 1-2 -> 2, 3-4 -> 4, 5-6 -> 6
        int tableSize = newCount <= 2 ? 2 : newCount <= 4 ? 4 : 6;

        bool success = _reservationsLogic.ChangeReservationPersons(reservationId, tableSize);
        if (success)
        {
            Console.WriteLine($"Guest count updated successfully! (Table for {tableSize} assigned)");
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
                Console.WriteLine("Reservation cancelled successfully!");
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

    private static void ViewFloorPlanForDate(string date)
    {
        TableAcces tableAccess = new TableAcces();
        List<TableModel> allTables = tableAccess.GetAllTables();
        List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(date, 0);

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine();
            ColorConsole.WriteTitle("╔═══════════════════════════════════════════════╗");
            ColorConsole.WriteTitle($"║       FLOOR PLAN - {date}              ║");
            ColorConsole.WriteTitle("╚═══════════════════════════════════════════════╝");
            Console.WriteLine();

            DisplayFloorPlanLegend();
            Console.WriteLine();

            DisplayAdminFloorPlan(allTables, reservedTableIds);

            Console.WriteLine();
            ColorConsole.WriteInfo("  Press ESC to return");

            key = Console.ReadKey(true).Key;

        } while (key != ConsoleKey.Escape);
    }

    private static void DisplayFloorPlanLegend()
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("█ Available  ");
        Console.ResetColor();
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("█ Reserved");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void DisplayAdminFloorPlan(List<TableModel> allTables, List<int> reservedTableIds)
    {
        const int floorWidth = 60;
        const int floorHeight = 25;

        var tablePositions = new Dictionary<int, (int Row, int Col)>
        {
            { 1, (2, 3) },
            { 2, (2, 10) },
            { 3, (2, 17) },
            { 4, (2, 24) },
            { 5, (7, 3) },
            { 6, (7, 12) },
            { 7, (7, 21) },
            { 8, (7, 30) },
            { 9, (7, 39) },
            { 10, (7, 48) },
            { 11, (12, 3) },
            { 12, (12, 14) },
            { 13, (12, 25) },
            { 14, (12, 36) }
        };

        string[,] grid = new string[floorHeight, floorWidth];
        ConsoleColor[,] colorGrid = new ConsoleColor[floorHeight, floorWidth];

        for (int i = 0; i < floorHeight; i++)
        {
            for (int j = 0; j < floorWidth; j++)
            {
                grid[i, j] = " ";
                colorGrid[i, j] = ConsoleColor.Black;
            }
        }

        foreach (var table in allTables)
        {
            if (tablePositions.ContainsKey(table.TableNumber))
            {
                var (row, col) = tablePositions[table.TableNumber];
                
                int boxWidth = table.TableCapacity + 2;
                int boxHeight = 3;
                
                if (row >= 0 && row < floorHeight - boxHeight && col >= 0 && col < floorWidth - boxWidth)
                {
                    bool isReserved = reservedTableIds.Contains(table.ID);
                    ConsoleColor tableColor = isReserved ? ConsoleColor.Red : ConsoleColor.White;
                    string capacity = $"{table.TableCapacity}p";

                    grid[row, col] = "┌";
                    for (int i = 1; i < boxWidth - 1; i++)
                    {
                        grid[row, col + i] = "─";
                    }
                    grid[row, col + boxWidth - 1] = "┐";

                    grid[row + 1, col] = "│";
                    int paddingLeft = (boxWidth - 2 - capacity.Length) / 2;
                    for (int i = 1; i < boxWidth - 1; i++)
                    {
                        if (i == paddingLeft + 1 && capacity.Length >= 2)
                        {
                            grid[row + 1, col + i] = capacity[0].ToString();
                        }
                        else if (i == paddingLeft + 2 && capacity.Length >= 2)
                        {
                            grid[row + 1, col + i] = capacity[1].ToString();
                        }
                        else
                        {
                            grid[row + 1, col + i] = " ";
                        }
                    }
                    grid[row + 1, col + boxWidth - 1] = "│";

                    grid[row + 2, col] = "└";
                    for (int i = 1; i < boxWidth - 1; i++)
                    {
                        grid[row + 2, col + i] = "─";
                    }
                    grid[row + 2, col + boxWidth - 1] = "┘";

                    for (int i = 0; i < boxHeight; i++)
                    {
                        for (int j = 0; j < boxWidth; j++)
                        {
                            colorGrid[row + i, col + j] = tableColor;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < floorHeight; i++)
        {
            Console.Write("  ");
            for (int j = 0; j < floorWidth; j++)
            {
                Console.ForegroundColor = colorGrid[i, j];
                Console.Write(grid[i, j]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    private static void ChangeReservationTimeForSingle(ReservationModel reservation)
    {
        Console.Clear();
        Console.WriteLine("\n=== Change Reservation Time ===\n");
        Console.WriteLine($"Guest: {reservation.GuestFirstName} {reservation.GuestLastName}");
        Console.WriteLine($"Current date/time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}");
        Console.Write("\nEnter new date/time (dd-MM-YYYY HH:mm): ");
        
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("No input provided. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (!DateTime.TryParseExact(input, "dd-MM-yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newTime))
        {
            Console.WriteLine("Invalid date format. Please use DD-MM-YYYY HH:mm format. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (newTime <= DateTime.Now)
        {
            Console.WriteLine("Cannot set reservation time in the past. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationTime(reservation.ID, newTime);
        if (success)
        {
            Console.WriteLine("Reservation time updated successfully!");
        }
        else
        {
            Console.WriteLine("✗ Failed to update reservation time.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void ChangeGuestCountForSingle(ReservationModel reservation)
    {
        Console.Clear();
        Console.WriteLine("\n=== Change Guest Count ===\n");
        Console.WriteLine($"Guest: {reservation.GuestFirstName} {reservation.GuestLastName}");
        Console.WriteLine($"Current guest count: {reservation.GuestCount}");
        Console.WriteLine($"Table capacity: {reservation.TableCapacity}");
        Console.Write("\nEnter new number of guests (1-6): ");
        
        if (!int.TryParse(Console.ReadLine(), out int newCount))
        {
            Console.WriteLine("Invalid number. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (newCount < 1 || newCount > 6)
        {
            Console.WriteLine("Guest count must be between 1 and 6. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        // Round up to nearest table size: 1-2 -> 2, 3-4 -> 4, 5-6 -> 6
        int tableSize = newCount <= 2 ? 2 : newCount <= 4 ? 4 : 6;

        bool success = _reservationsLogic.ChangeReservationPersons(reservation.ID, tableSize);
        if (success)
        {
            Console.WriteLine($"Guest count updated successfully! (Table for {tableSize} assigned)");
        }
        else
        {
            Console.WriteLine("✗ Failed to update guest count.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void CancelSingleReservation(ReservationModel reservation)
    {
        Console.Clear();
        Console.WriteLine("\n=== Cancel Reservation ===\n");
        Console.WriteLine($"Guest: {reservation.GuestFirstName} {reservation.GuestLastName}");
        Console.WriteLine($"Email: {reservation.GuestEmail}");
        Console.WriteLine($"Date/time: {DateTime.Parse(reservation.StartAt):yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Table: {reservation.TableNumber} ({reservation.TableCapacity} seats)");
        Console.WriteLine($"Guest count: {reservation.GuestCount}");
        Console.Write("\nAre you sure you want to cancel this reservation? (y/n): ");
        
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Cancellation aborted.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.CancelReservation(reservation.ID);
        if (success)
        {
            Console.WriteLine("Reservation cancelled successfully!");
        }
        else
        {
            Console.WriteLine("✗ Failed to cancel reservation.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
