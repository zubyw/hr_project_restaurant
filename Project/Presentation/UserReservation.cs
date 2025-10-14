using System;

static class UserReservation
{

    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
    private static UsersLogic _usersLogic = new UsersLogic();

    public static void Start()
    {
        try
        {
            Console.Clear();
        Console.WriteLine("\n===Reservations===");

        Console.WriteLine("Amount of people: (1-6)");
        string? AmountPeople = Console.ReadLine();
        if (string.IsNullOrEmpty(AmountPeople))
        {
            Console.WriteLine("All fields are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }
        if (!UserMakeReservationLogic.CheckAmountPeople(AmountPeople))
        {
            Console.WriteLine("Given amount of people incorrect (1-6)");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        Console.WriteLine("Date: (YYYY-MM-DD)");
        string? ReservationDate = Console.ReadLine();
        if (string.IsNullOrEmpty(ReservationDate))
        {
            Console.WriteLine("All fields are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }
        if (!UserMakeReservationLogic.CheckValidDate(ReservationDate))
        {
            Console.WriteLine("Given date format incorrect (YYYY-MM-DD)");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        Console.WriteLine("Arrival Time: (Choose From 17:00 - 17:30 - 18:00 - 18:30 - 19:00 - 19:30)");
        string? ArrivalTime = Console.ReadLine();
        if (string.IsNullOrEmpty(ArrivalTime))
        {
            Console.WriteLine("All fields are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }
        if (!UserMakeReservationLogic.CheckValidDayTime(ArrivalTime))
        {
            Console.WriteLine("Given daytime incorrect (17:00 - 17:30 - 18:00 - 18:30 - 19:00 - 19:30)");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        int DiningTableSize = UserMakeReservationLogic.GetTableSize(AmountPeople);
        TableModel? AvailableTable = UserMakeReservationLogic.GetAvailableTable(ReservationDate, DiningTableSize);
        
        if (AvailableTable == null)
        {
            Console.WriteLine("Sorry, no tables are available for the selected date and time.");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }
        
        int.TryParse(AmountPeople, out int intAmountPeople);
        string CompleteStartDate = $"{ReservationDate} {ArrivalTime}:00";
        
        if (Menu.CurrentUser == null)
        {
            Console.WriteLine("User not logged in. Please login first.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Menu.Start();
            return;
        }
        
        int userid = _usersLogic.GetIdByEmail(Menu.CurrentUser.EmailAddress);

        if (_reservationsLogic.CreateReservation(userid, AvailableTable.ID, intAmountPeople, CompleteStartDate))
        {
            Console.WriteLine("Reservation successful. Redirecting to main menu...");

            // Small delay to show the Reservation succesful message
            Thread.Sleep(1500);

            Menu.ShowMainMenu();
        }
        else
        {
            Console.WriteLine("An unexpected error occurred");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Menu.ShowMainMenu();
        }
    }
    catch
    {
        Console.WriteLine("An unexpected error occurred");
        Console.WriteLine("Press any key to try again...");
        Console.ReadKey();
        Menu.ShowMainMenu();
    }
    }
}