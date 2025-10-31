using System;
using Project.DataModels;

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
        Console.WriteLine();
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
        Console.Clear();
        Console.WriteLine("\n===Reservations===");
        Console.WriteLine();
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

        Console.WriteLine("Select Arrival Time:");
        string ArrivalTime = _reservationsLogic.SelectArrivalTime();
        Console.WriteLine($"You selected: {ArrivalTime}");

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
        
        Console.WriteLine("Make a dish selection? (Y/N)");
        string? MakesDishSelection = Console.ReadLine()?.Trim().ToUpper();
        
        if (string.IsNullOrEmpty(MakesDishSelection))
        {
            Console.WriteLine("All fields are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }
            if (MakesDishSelection == "Y")
            {
                // ===== DISH SELECTION STEP =====
                // Get current theme
                var dishLogic = new DishLogic();
                int? currentThemeId = dishLogic.GetCurrentThemeId();

                List<DishModel> selectedDishes = new List<DishModel>();

                if (currentThemeId.HasValue)
                {
                    // Show dish selection menu
                    selectedDishes = DishSelection.SelectDishesForReservation(intAmountPeople, currentThemeId.Value);

                    if (selectedDishes.Count == 0)
                    {
                        // User cancelled or something went wrong
                        ColorConsole.WriteWarning("Dish selection cancelled. Returning to main menu...");
                        Thread.Sleep(1500);
                        Menu.ShowMainMenu();
                        return;
                    }
                }
                else
                {
                    ColorConsole.WriteWarning("No theme available for the current month. Proceeding without dish selection...");
                    Thread.Sleep(2000);
                }

                // ===== CREATE RESERVATION =====
                int userid = _usersLogic.GetIdByEmail(Menu.CurrentUser.EmailAddress);

                if (_reservationsLogic.CreateReservation(userid, AvailableTable.ID, intAmountPeople, CompleteStartDate))
                {
                    // Get the reservation ID
                    var userReservations = _reservationsLogic.GetReservationsByUserId(userid);
                    var newReservation = userReservations.OrderByDescending(r => r.ID).FirstOrDefault();

                    // Save selected dishes to the reservation
                    if (newReservation != null && selectedDishes.Count > 0)
                    {
                        try
                        {
                            dishLogic.ReserveDishes(selectedDishes, newReservation);
                        }
                        catch (Exception ex)
                        {
                            ColorConsole.WriteError($"Warning: Could not save dish selections: {ex.Message}");
                            Thread.Sleep(2000);
                        }
                    }

                    // Success message already shown in DishSelection, just redirect
                    Thread.Sleep(500);
                    Menu.ShowMainMenu();
                    return;
                }
                else
                {
                    Console.WriteLine("An unexpected error occurred creating the reservation");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    Menu.ShowMainMenu();
                    return;
                }
            }
            else if (MakesDishSelection == "N")
            {
                int userid = _usersLogic.GetIdByEmail(Menu.CurrentUser.EmailAddress);
                if (_reservationsLogic.CreateReservation(userid, AvailableTable.ID, intAmountPeople, CompleteStartDate))
                {
                    Console.WriteLine("✅ Your reservation has been saved!");
                    Thread.Sleep(3000);
                    Menu.ShowMainMenu();
                    return;
                }
                else
                {
                    Console.WriteLine("An unexpected error occurred creating the reservation");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    Menu.ShowMainMenu();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid input! Please enter Y or N.");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                Start();
                return;
            }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        Console.WriteLine("Press any key to try again...");
        Console.ReadKey();
        Menu.ShowMainMenu();
    }
    }
}