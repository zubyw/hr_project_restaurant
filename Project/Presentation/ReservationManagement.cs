using System;
using System.Collections.Generic;
using Project.DataModels;
using Project.Logic;
using Project.Presentation;

static class ReservationManagement
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
    private static RudReservation _rudReservation = new RudReservation();

    public static void Start()
    {
        bool running = true;

        while (running)
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
                Console.WriteLine("\nUse ↑/↓ to navigate, Enter to update, Esc to go back");

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
                        _rudReservation.Start();
                        reservations = _reservationsLogic.GetAllReservations();
                        break;

                    case ConsoleKey.Escape:
                        selecting = false;
                        running = false;
                        break;
                }
            }
        }
    }
}
