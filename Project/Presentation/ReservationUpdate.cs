using System;

static class ReservationUpdateMenu
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n=== Change Reservations ===");
        Console.WriteLine("1. Change reservation time");
        Console.WriteLine("2. Change number of guests");
        Console.WriteLine("3. Cancel reservation");
        Console.WriteLine("4. Back");
        Console.Write("Select an option: ");
        string? input = Console.ReadLine();

        switch (input)
        {
            case "1":
                ChangeReservationTime();
                break;
            case "2":
                ChangeGuestCount();
                break;
            case "3":
                CancelReservation();
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue...");
                Console.ReadKey();
                Start();
                break;
        }
    }

    private static void ChangeReservationTime()
    {
        Console.Clear();
        Console.Write("Enter reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to return...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter new date/time (YYYY-MM-DD HH:mm): ");
        string? input = Console.ReadLine();

        if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newTime))
        {
            Console.WriteLine("Invalid date format. Press any key to return...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationTime(reservationId, newTime);
        Console.WriteLine(success ? "Reservation time updated successfully." : "Failed to update reservation time.");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    private static void ChangeGuestCount()
    {
        Console.Clear();
        Console.Write("Enter reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to return...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter new number of guests (2, 4, or 6): ");
        if (!int.TryParse(Console.ReadLine(), out int newCount))
        {
            Console.WriteLine("Invalid number. Press any key to return...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.ChangeReservationPersons(reservationId, newCount);
        Console.WriteLine(success ? "Guest count updated successfully." : "Failed to update guest count.");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    private static void CancelReservation()
    {
        Console.Clear();
        Console.Write("Enter reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        {
            Console.WriteLine("Invalid ID. Press any key to return...");
            Console.ReadKey();
            return;
        }

        bool success = _reservationsLogic.CancelReservation(reservationId);
        Console.WriteLine(success ? "Reservation cancelled." : "Failed to cancel reservation.");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }
}
