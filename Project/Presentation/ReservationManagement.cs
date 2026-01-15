using System;
using System.Collections.Generic;
using Project.DataModels;
using Project.Logic;
using Project.Presentation;

static class ReservationManagement
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
    private static RudReservation _rudReservation = new RudReservation();
    private static Tablelogic _tableLogic = new Tablelogic();

    public static void Start()
    {
        bool running = true;

        while (running)
        {
            // Herlaad altijd de volledige lijst van reserveringen
            List<ReservationModel> reservations = _reservationsLogic.GetAllReservations();

            if (reservations.Count == 0)
            {
                Console.Clear();
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
                Console.WriteLine("=== Admin Reservation Panel ===\n");

                Console.WriteLine("┌──────┬─────────┬──────────────────────────────┬───────┬─────────────────────┬───────────┐");
                Console.WriteLine("│  ID  │  Table  │          Guest Name          │ Count │      Date/Time      │  Status   │");
                Console.WriteLine("├──────┼─────────┼──────────────────────────────┼───────┼─────────────────────┼───────────┤");

                for (int i = 0; i < reservations.Count; i++)
                {
                    ReservationModel r = reservations[i];
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
                Console.WriteLine("\nUse ↑/↓ to navigate, Enter to manage, Esc to go back");

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

                        // Herlaad de lijst na een actie en reset de selectie
                        reservations = _reservationsLogic.GetAllReservations();
                        selectedIndex = 0;

                        // Als er geen reserveringen meer zijn, terug naar hoofdmenu
                        if (reservations.Count == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("No reservations found.");
                            Console.WriteLine("Press any key to return...");
                            Console.ReadKey();
                            running = false;
                            selecting = false;
                        }
                        break;

                    case ConsoleKey.Escape:
                        selecting = false;
                        running = false;
                        break;
                }
            }
        }
    }

    private static void ManageReservation(ReservationModel selectedReservation)
    {
        selectedReservation = _reservationsLogic.ReloadReservation(selectedReservation);
        bool managing = true;
        string[] manageOptions = { "Update reservation", "Cancel reservation", "Back" };
        int manageIndex = 0;

        while (managing)
        {
            Console.Clear();
            Console.WriteLine($"=== Manage Reservation ===\n");
            Console.WriteLine($"ID: {selectedReservation.ID}");
            Console.WriteLine($"Table: {selectedReservation.TableId} ({_tableLogic.ReturnTableSize(selectedReservation)})");
            Console.WriteLine($"Guest Count: {selectedReservation.GuestCount}");
            Console.WriteLine($"Date/Time: {DateTime.Parse(selectedReservation.StartAt):dd-MM-yyyy HH:mm}");
            Console.WriteLine($"Status: {selectedReservation.Status}\n");

            Console.WriteLine("Use ↑/↓ to navigate and Enter to select:\n");

            for (int i = 0; i < manageOptions.Length; i++)
            {
                bool isSelected = i == manageIndex;
                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"  {manageOptions[i]}");

                if (isSelected)
                    Console.ResetColor();
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    manageIndex = (manageIndex - 1 + manageOptions.Length) % manageOptions.Length;
                    break;

                case ConsoleKey.DownArrow:
                    manageIndex = (manageIndex + 1) % manageOptions.Length;
                    break;

                case ConsoleKey.Enter:
                    switch (manageIndex)
                    {
                        case 0: // Update
                            _rudReservation.Update(selectedReservation);
                            selectedReservation = _reservationsLogic.ReloadReservation(selectedReservation);
                            break;

                        case 1: // Cancel
                            _rudReservation.Delete(selectedReservation);
                            managing = false; // terug naar lijst na cancel
                            break;

                        case 2: // Back
                            managing = false; // terug naar lijst
                            break;
                    }
                    break;

                case ConsoleKey.Escape:
                    managing = false;
                    break;
            }
        }
    }
}
