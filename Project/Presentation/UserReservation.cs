using System;
using Project.DataModels;
using Project.Logic;
using Project.Presentation;
using System.Globalization; // <- toegevoegd voor DateTime parsing

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
            string? ReservationDate = CalanderInput.Calander();

            Console.WriteLine("Select Arrival Time:");
            string ArrivalTime = _reservationsLogic.SelectArrivalTime();
            Console.WriteLine($"You selected: {ArrivalTime}");

            int.TryParse(AmountPeople, out int intAmountPeople);

            // Get all tables and reserved tables for floor plan
            TableAcces tableAccess = new TableAcces();
            List<TableModel> allTables = tableAccess.GetAllTables();
            List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(ReservationDate, intAmountPeople);

            // Show floor plan and let user select a table
            TableModel? AvailableTable = FloorPlanView.SelectTableFromFloorPlan(allTables, reservedTableIds, intAmountPeople);

            if (AvailableTable == null)
            {
                Console.WriteLine("Table selection cancelled.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                Start();
                return;
            }

            // Combine date en tijd in dd-MM-yyyy HH:mm formaat
            string CompleteStartDate = ReservationDate + " " + ArrivalTime;

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
                var dishLogic = new DishLogic();
                ThemeModel? correctTheme = dishLogic.GetCorrectTheme(CompleteStartDate);

                List<DishModel> selectedDishes = new List<DishModel>();

                if (correctTheme is not null)
                {
                    selectedDishes = DishSelection.SelectDishesForReservation(intAmountPeople, correctTheme);

                    if (selectedDishes.Count == 0)
                    {
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

                int userid = _usersLogic.GetIdByEmail(Menu.CurrentUser.EmailAddress);

                if (_reservationsLogic.CreateReservation(userid, AvailableTable.ID, intAmountPeople, CompleteStartDate))
                {
                    var userReservations = _reservationsLogic.GetReservationsByUserId(userid);
                    var newReservation = userReservations.OrderByDescending(r => r.ID).FirstOrDefault();

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
                    Console.WriteLine("Your reservation has been saved!");
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
