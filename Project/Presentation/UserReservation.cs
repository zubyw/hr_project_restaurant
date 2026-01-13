using System;
using Project.DataModels;
using Project.Logic;
using Project.Presentation;
using System.Globalization;
using System.Net;

static class UserReservation
{
    private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
    private static UsersLogic _usersLogic = new UsersLogic();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n===Reservations===\n");

        int intAmountPeople = GetAmountOfPeople();
        if (intAmountPeople == -1) return;

        string reservationDate = GetReservationDate();
        if (reservationDate == null) return;

        string arrivalTime = GetReservationTime();

        string completeStartDate = reservationDate + " " + arrivalTime;

        TableModel? selectedTable = SelectTable(reservationDate, intAmountPeople);
        if (selectedTable == null) return;

        bool? makeDishSelection = AskForDishSelection();
        if (makeDishSelection == null) return;

        int userId = _usersLogic.GetIdByEmail(Menu.CurrentUser.EmailAddress);

        ReservationModel reservation = new ReservationModel();
        reservation.UserId = userId;
        reservation.TableId = selectedTable.ID;
        reservation.GuestCount = intAmountPeople;
        reservation.StartAt = completeStartDate;
        reservation.Status = "Open";
        reservation.CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        bool reservationSuccess = _reservationsLogic.CreateReservation(reservation);

        if (!reservationSuccess)
        {
            ShowRetryMessage("An unexpected error occurred creating the reservation");
            return;
        }

        if (makeDishSelection.Value)
        {
            HandleDishSelection(intAmountPeople, completeStartDate, userId);
        }
        else
        {
            Console.WriteLine("Your reservation has been saved!");
            Thread.Sleep(2000);
            Menu.ShowMainMenu();
        }
    }

    private static int GetAmountOfPeople()
    {
        Console.WriteLine("Amount of people: (1-6)");
        string? amountPeople = Console.ReadLine();

        if (string.IsNullOrEmpty(amountPeople))
        {
            ShowRetryMessage("All fields are required!");
            return -1;
        }

        if (!UserMakeReservationLogic.CheckAmountPeople(amountPeople))
        {
            ShowRetryMessage("Given amount of people incorrect (1-6)");
            return -1;
        }

        int.TryParse(amountPeople, out int intAmountPeople);
        return intAmountPeople;
    }

    private static string GetReservationDate()
    {
        Console.Clear();
        Console.WriteLine("\n===Reservations===\n");
        string? reservationDate = CalanderInput.Calander();

        if (string.IsNullOrEmpty(reservationDate))
        {
            ShowRetryMessage("Reservation date is required!");
            return null;
        }

        return reservationDate;
    }

    private static string GetReservationTime()
    {
        Console.WriteLine("Select Arrival Time:");
        string arrivalTime = _reservationsLogic.SelectArrivalTime();
        Console.WriteLine($"You selected: {arrivalTime}");
        return arrivalTime;
    }

    private static TableModel? SelectTable(string reservationDate, int AmountPeople)
    {
        TableAccess tableAccess = new TableAccess();
        List<TableModel> allTables = tableAccess.GetAllTables();
        List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(reservationDate, AmountPeople);

        TableModel? selectedTable = FloorPlanView.SelectTableFromFloorPlan(allTables, reservedTableIds, AmountPeople);
        if (selectedTable is null)
        {
            Console.Clear();
            Console.WriteLine($"Party Size: {AmountPeople}");
            Console.WriteLine();
            Console.WriteLine("No tables available for that size this day.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
        return selectedTable;
    }

    private static bool? AskForDishSelection()
    {
        Console.WriteLine("Make a dish selection? (Y/N)");
        string? input = Console.ReadLine()?.Trim().ToUpper();

        if (string.IsNullOrEmpty(input))
        {
            ShowRetryMessage("All fields are required!");
            return null;
        }

        if (input == "Y") return true;
        if (input == "N") return false;

        ShowRetryMessage("Invalid input! Please enter Y or N.");
        return null;
    }

    private static void HandleDishSelection(int amountPeople, string completeStartDate, int userId)
    {
        var dishLogic = new DishLogic();
        ThemeModel? correctTheme = dishLogic.GetCorrectTheme(completeStartDate);

        if (correctTheme == null)
        {
            ColorConsole.WriteWarning("No theme available for the current month. Proceeding without dish selection...");
            Thread.Sleep(2000);
            Menu.ShowMainMenu();
            return;
        }

        List<DishModel> selectedDishes = DishSelection.SelectDishesForReservation(amountPeople, correctTheme);

        if (selectedDishes.Count == 0)
        {
            ColorConsole.WriteWarning("Dish selection cancelled. Returning to main menu...");
            Thread.Sleep(1500);
            Menu.ShowMainMenu();
            return;
        }

        var userReservations = _reservationsLogic.GetReservationsByUserId(userId); // all reservations by user
        var newReservation = userReservations.OrderByDescending(r => r.ID).FirstOrDefault();// latest reservation made by user

        if (newReservation != null)
        {
            dishLogic.ReserveDishes(selectedDishes, newReservation);
        }

        Console.WriteLine("Your reservation and dish selection have been saved!");
        Thread.Sleep(2000);
        Menu.ShowMainMenu();
    }

    private static void ShowRetryMessage(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine("Press any key to try again...");
        Console.ReadKey();
        Start();
    }
}
