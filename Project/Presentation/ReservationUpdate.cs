using System;
using Project.DataModels;
using Project.Logic;

static class ReservationUpdateMenu
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();

    // ensure reservation exists and belongs to current user
    private static bool ReservationExistsForCurrentUser(int reservationId)
    {
        ReservationModel? r = _reservationsLogic.GetReservationById(reservationId);
        if (r == null) return false;

        if (ReservationsLogic.CurrentUserId.HasValue &&
            r.UserId != ReservationsLogic.CurrentUserId.Value)
        {
            return false;
        }
        return true;
    }

    // Main update menu
    public static void Start()
    {
        string[] options = new string[]
        {
        "Change reservation time",
        "Change number of guests",
        "Cancel reservation",
        "Back"
        };

        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("\n=== Change Reservations ===");

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
                            ChangeReservationTime();
                            break;
                        case 1:
                            ChangeGuestCount();
                            break;
                        case 2:
                            CancelReservation();
                            break;
                        case 3:
                            return; // Back
                    }
                    break;
            }

        } while (key != ConsoleKey.Enter || selectedIndex != 3); // Blijf menu tonen tot gebruiker op "Back" Enter drukt
    }


    // Change time → date then arrow-time (no typing HH:mm)
    private static void ChangeReservationTime()
    {
        Console.Clear();
        Console.Write("Enter reservation ID: ");
        int reservationId;
        if (!int.TryParse(Console.ReadLine(), out reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to return...");
            Console.ReadKey();
            return;
        }

        if (!ReservationExistsForCurrentUser(reservationId))
        {
            Console.WriteLine("Reservation not found for this user.");
            Console.ReadKey();
            return;
        }

        ReservationModel? rcheck = _reservationsLogic.GetReservationById(reservationId);
        if (rcheck != null && !_reservationsLogic.CanModifyOrCancel(rcheck))
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Reservations within 24 hours cannot be modified or canceled.");
            Console.ResetColor();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter date (DD-MM-YYYY): ");
        string? dateIn = Console.ReadLine();
        DateTime dateOnly;
        if (string.IsNullOrEmpty(dateIn) ||
            !DateTime.TryParseExact(dateIn, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out dateOnly))
        {
            Console.WriteLine("Invalid date. Please use DD-MM-YYYY format. Press any key to return...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Select Arrival Time:");
        string selectedTime = SelectArrivalTimeMenu(); // "HH:mm"

        string combined = $"{dateOnly:dd-MM-yyyy} {selectedTime}";
        DateTime newTime;
        if (!DateTime.TryParseExact(combined, "dd-MM-yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out newTime))
        {
            Console.WriteLine("Invalid date/time. Press any key to return...");
            Console.ReadKey();
            return;
        }

        if (!_reservationsLogic.IsValidReservationDateTime(combined))
        {
            Console.WriteLine("Invalid time (must be after 17:00). Press any key to return...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationTime(reservationId, newTime);
        Console.WriteLine(success ? "Reservation time updated successfully." : "Failed to update reservation time.");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    // Change guests → enforce 1–6
    private static void ChangeGuestCount()
    {
        Console.Clear();
        Console.Write("Enter reservation ID: ");
        int reservationId;
        if (!int.TryParse(Console.ReadLine(), out reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to return...");
            Console.ReadKey();
            return;
        }

        if (!ReservationExistsForCurrentUser(reservationId))
        {
            Console.WriteLine("Reservation not found for this user.");
            Console.ReadKey();
            return;
        }

        ReservationModel? rcheck = _reservationsLogic.GetReservationById(reservationId);
        if (rcheck != null && !_reservationsLogic.CanModifyOrCancel(rcheck))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Reservations within 24 hours cannot be modified or canceled.");
            Console.ResetColor();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter new number of guests (1–6): ");
        int newCount;
        if (!int.TryParse(Console.ReadLine(), out newCount) || newCount < 1 || newCount > 6)
        {
            Console.WriteLine("Guest count must be between 1 and 6. Press any key to return...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationPersons(reservationId, newCount);
        Console.WriteLine(success ? "Guest count updated successfully." : "Failed to update guest count.");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    // Cancel reservation (only own)
    private static void CancelReservation()
    {
        Console.Clear();
        Console.Write("Enter reservation ID: ");
        int reservationId;
        if (!int.TryParse(Console.ReadLine(), out reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to return...");
            Console.ReadKey();
            return;
        }

        if (!ReservationExistsForCurrentUser(reservationId))
        {
            Console.WriteLine("Reservation not found for this user.");
            Console.ReadKey();
            return;
        }

        ReservationModel? rcheck = _reservationsLogic.GetReservationById(reservationId);
        if (rcheck != null && !_reservationsLogic.CanModifyOrCancel(rcheck))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Reservations within 24 hours cannot be modified or canceled.");
            Console.ResetColor();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return;
        }

        ReservationModel? reservation = _reservationsLogic.GetReservationById(reservationId);
        if (reservation != null)
        {
            Console.WriteLine($"Date/time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}");
        }

        bool success = _reservationsLogic.CancelReservation(reservationId);
        Console.WriteLine(success ? "Reservation cancelled." : "Failed to cancel reservation.");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    // Arrow-key time slot menu (same look as create flow)
    private static string SelectArrivalTimeMenu()
    {
        string[] timeSlots = new string[]
        {
            "17:00","17:30","18:00","18:30",
            "19:00","19:30","20:00","20:30"
        };

        int selectedIndex = 0;
        ConsoleKey key = ConsoleKey.NoName;

        while (key != ConsoleKey.Enter)
        {
            Console.Clear();
            Console.WriteLine("=== Reservations ===");
            Console.WriteLine("Select arrival time:");
            Console.WriteLine();

            for (int i = 0; i < timeSlots.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("> ");
                    Console.WriteLine(timeSlots[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("  ");
                    Console.WriteLine(timeSlots[i]);
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                if (selectedIndex == 0) selectedIndex = timeSlots.Length - 1;
                else selectedIndex = selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % timeSlots.Length;
            }
        }

        return timeSlots[selectedIndex];
    }
}
