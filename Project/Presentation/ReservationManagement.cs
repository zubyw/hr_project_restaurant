using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Project.DataModels;
using Project.Logic;
using Project.Presentation;

static class ReservationManagement
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
    private static DishLogic _dishLogic = new DishLogic();

    private static RudReservation rruservation = new RudReservation();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("=== Admin Reservation Panel ===\n");

        List<ReservationModel> reservations = _reservationsLogic.GetAllReservations();

        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations found.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return;
        }

        int selectedIndex = 0;
        bool selecting = true;

        while (selecting)
        {
            Console.Clear();
            Console.WriteLine("┌──────┬─────────┬──────────────────────────────┬───────┬─────────────────────┬───────────┐");
            Console.WriteLine("│  ID  │  Table  │          Guest Name          │ Count │      Date/Time      │  Status   │");
            Console.WriteLine("├──────┼─────────┼──────────────────────────────┼───────┼─────────────────────┼───────────┤");

            for (int i = 0; i < reservations.Count; i++)
            {
                var r = reservations[i];
                string guestName = $"{r.GuestFirstName} {r.GuestLastName}";
                string dateTime = DateTime.Parse(r.StartAt).ToString("dd-MM-yyyy HH:mm");

                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"│ {r.ID,4} │ {r.TableNumber,2} ({r.TableCapacity,2}) │ {guestName,-28} │ {r.GuestCount,3}   │ {dateTime,-19} │ {r.Status,-9} │");

                if (i == selectedIndex)
                    Console.ResetColor();
            }

            Console.WriteLine("└──────┴─────────┴──────────────────────────────┴───────┴─────────────────────┴───────────┘");
            Console.WriteLine("\nUse ↑/↓ to navigate, Enter to select, Esc to go back");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + reservations.Count) % reservations.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % reservations.Count;
                    break;
                case ConsoleKey.Enter:
                    ManageReservation(reservations[selectedIndex]);
                    reservations = _reservationsLogic.GetAllReservations(); // refresh
                    break;
                case ConsoleKey.Escape:
                    selecting = false;
                    break;
            }
        }
    }

    private static void ManageReservation(ReservationModel reservation)
    {
        string[] options = { "Table Selection", "Guest Count", "Date & Time", "Dish Selection", "Cancel Reservation", "View Dish Orders", "Back" };
        int selectedIndex = 0;
        bool managing = true;

        while (managing)
        {
            Console.Clear();
            Console.WriteLine("=== Manage Reservation ===\n");
            Console.WriteLine($"ID: {reservation.ID} | Table: {reservation.TableNumber} ({reservation.TableCapacity}) | Guests: {reservation.GuestCount} | Date/Time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm} | Status: {reservation.Status}\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"  {options[i]}");
                if (i == selectedIndex) Console.ResetColor();
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
                        case 0: EditTableSelection(reservation); break;
                        case 1: EditGuestCount(reservation); break;
                        case 2: EditDateTime(reservation); break;
                        case 3: EditDishSelection(reservation); break;
                        case 4: CancelReservation(reservation); managing = false; break;
                        case 5: DishOrderOverview.Start(); break;
                        case 6: managing = false; break;
                    }
                    reservation = _reservationsLogic.ReloadReservation(reservation);
                    break;
                case ConsoleKey.Escape:
                    managing = false;
                    break;
            }
        }
    }

    private static void EditTableSelection(ReservationModel reservation)
    {
        TableAcces tableAccess = new TableAcces();
        List<TableModel> allTables = tableAccess.GetAllTables();
        List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(reservation.StartAt, reservation.GuestCount)
                                              .Where(id => id != reservation.TableId)
                                              .ToList();

        TableModel? selectedTable = FloorPlanView.SelectTableFromFloorPlan(allTables, reservedTableIds, reservation.GuestCount);

        if (selectedTable != null && !reservedTableIds.Contains(selectedTable.ID))
        {
            reservation.TableId = selectedTable.ID;
            _reservationsLogic.UpdateReservation(reservation);
            Console.WriteLine($"Table changed to {selectedTable.ID}. Press any key to continue...");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Cannot select a reserved table. Press any key to return...");
            Console.ReadKey();
        }
    }

    private static void EditGuestCount(ReservationModel reservation)
    {
        Console.Clear();
        Console.WriteLine($"Current guest count: {reservation.GuestCount}");
        Console.Write("Enter new guest count (1-6): ");
        if (int.TryParse(Console.ReadLine(), out int newCount) && newCount >= 1 && newCount <= 6)
        {
            reservation.GuestCount = newCount;
            _reservationsLogic.UpdateReservation(reservation);
            reservation.GuestCount = newCount;
            Console.WriteLine("Guest count updated. Please select a new table to fit the guest count...");
            Console.ReadKey();

            EditTableSelection(reservation);
        }
        else
        {
            Console.WriteLine("Invalid input. Press any key to return...");
            Console.ReadKey();
        }
    }

    private static void EditDateTime(ReservationModel reservation)
    {
        Console.Clear();
        Console.WriteLine($"Current date/time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}");
        Console.Write("Enter new date/time (dd-MM-yyyy HH:mm): ");
        string? input = Console.ReadLine();

        if (DateTime.TryParseExact(input, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out DateTime newTime))
        {
            reservation.StartAt = newTime.ToString("dd-MM-yyyy HH:mm");
            _reservationsLogic.UpdateReservation(reservation);
            Console.WriteLine("Reservation time updated. Press any key to continue...");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Invalid format. Press any key to return...");
            Console.ReadKey();
        }
    }

    private static void EditDishSelection(ReservationModel reservation)
    {
        bool hasDishes = _reservationsLogic.ReservationContainsDishes(reservation);
        Console.Clear();
        Console.WriteLine(hasDishes ? "Dish selection already made." : "No dishes selected yet.");

        string[] options = hasDishes ? new[] { "Change dish selection", "Remove dish selection", "Back" } : new[] { "Add dish selection", "Back" };
        int index = 0;
        bool choosing = true;

        while (choosing)
        {
            Console.Clear();
            Console.WriteLine("=== Edit Dish Selection ===\n");
            for (int i = 0; i < options.Length; i++)
            {
                if (i == index)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"  {options[i]}");
                if (i == index) Console.ResetColor();
            }

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow: index = (index - 1 + options.Length) % options.Length; break;
                case ConsoleKey.DownArrow: index = (index + 1) % options.Length; break;
                case ConsoleKey.Enter:
                    if ((options[index].Contains("Change") || options[index].Contains("Add")))
                    {
                        ThemeModel? theme = _dishLogic.GetCorrectTheme(reservation.StartAt);
                        if (theme is not null)
                        {
                            List<DishModel> selectedDishes = DishSelection.SelectDishesForReservation(reservation.GuestCount, theme);
                            _dishLogic.ReserveDishes(selectedDishes, reservation, true);
                        }
                    }
                    else if (options[index].Contains("Remove"))
                    {
                        _dishLogic.DeleteDishesFromReservation(reservation);
                    }
                    choosing = false;
                    break;
                case ConsoleKey.Escape:
                    choosing = false;
                    break;
            }
        }
    }

    private static void CancelReservation(ReservationModel reservation)
    {
        Console.Write($"Are you sure you want to cancel reservation {reservation.ID}? (Y/N): ");
        string? input = Console.ReadLine()?.Trim().ToUpper();
        if (input == "Y")
        {
            _reservationsLogic.CancelReservation(reservation.ID);
            Console.WriteLine("Reservation cancelled. Press any key to continue...");
            Console.ReadKey();
        }
    }
}
