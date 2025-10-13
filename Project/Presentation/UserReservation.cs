using System.ComponentModel.DataAnnotations.Schema;

static class UserMakeReservation
{

    public static void Start()
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
        if (!UserMakeReservationLogic.CheckValidDate(ReservationDate))
        {
            Console.WriteLine("Given daytime incorrect (17:00 - 17:30 - 18:00 - 18:30 - 19:00 - 19:30)");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        int DiningTableSize = UserMakeReservationLogic.GetTableSize(AmountPeople);
        TableModel Availabe = UserMakeReservationLogic.GetAvailableTable(ReservationDate, DiningTableSize);
        int.TryParse(AmountPeople, out int intAmountPeople);
        string CompleteStartDate = ReservationDate + ArrivalTime;
    }
}