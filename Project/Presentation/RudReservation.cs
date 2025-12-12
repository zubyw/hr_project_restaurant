using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using Project.DataModels;
using Project.Logic;

namespace Project.Presentation
{
    public class RudReservation
    {
        // Logic layer instance
        private static ReservationsLogic _reservationsLogic = new ReservationsLogic();
        private static DishLogic _dishLogic = new DishLogic();
        // Starts the reservation menu
        public void Start()
        {
            // needed for ownership checks in logic
            ReservationsLogic.CurrentUserId = UserLogin.activeUser.ID;

            Console.Clear();
            Console.WriteLine("=== My Reservations ===");
            Console.WriteLine();

            // Load all reservations for the logged-in user
            List<ReservationModel> reservations = _reservationsLogic.GetReservationsByUserIdForGuest(UserLogin.activeUser.ID);

            // If none, go back to where the function was caled 
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
                Console.WriteLine();
                Console.WriteLine("┌────────┬────────┬─────────────────────┬────────────┐");
                Console.WriteLine("│ Table  │ Guests │      Date/Time      │   Status   │");
                Console.WriteLine("├────────┼────────┼─────────────────────┼────────────┤");

                for (int i = 0; i < reservations.Count; i++)
                {
                    var r = reservations[i];
                    string dateTime = DateTime.ParseExact(r.StartAt, "dd-MM-yyyy HH:mm", null).ToString();
                    bool isSelected = (i == selectedIndex);

                    if (isSelected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine($"│ {r.TableId,6} │ {r.GuestCount,6} │ {dateTime,-19} │ {r.Status,-10} │");

                    if (isSelected)
                        Console.ResetColor();
                }

                Console.WriteLine("└────────┴────────┴─────────────────────┴────────────┘");
                Console.WriteLine();
                Console.WriteLine("↑/↓ to navigate, Enter to select, Esc to go back");

                // Handle input
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + reservations.Count) % reservations.Count;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % reservations.Count;
                        break;

                    case ConsoleKey.Enter:
                        selecting = false;
                        break;

                    case ConsoleKey.Escape:
                        Menu.ShowCustomerMenu();
                        return;
                }
            }

            // After user presses Enter:
            ReservationModel selectedReservation = reservations[selectedIndex];

            if (_reservationsLogic.IsReservationCanceled(selectedReservation))
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Unable to update a reservation that is canceled");
                Thread.Sleep(1500);
                Start();
            }

            // Manage that reservation
            string[] manageOptions = { "Update reservation", "Cancel reservation", "Back" };
            int manageIndex = 0;
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine($"=== Manage Reservation ===\n");
                Console.WriteLine($"Table: {selectedReservation.TableId}");
                Console.WriteLine($"Guests: {selectedReservation.GuestCount}");
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

                var key = Console.ReadKey(true);

                switch (key.Key)
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
                                Update(selectedReservation);
                                selectedReservation = _reservationsLogic.ReloadReservation(selectedReservation);
                                break;

                            case 1: // Cancel
                                Delete(selectedReservation);
                                break;

                            case 2: // Back
                                managing = false;
                                Start();
                                break;
                        }
                        break;

                    case ConsoleKey.Escape:
                        managing = false;
                        break;
                }
            }
        }

        // Update a reservation (same step flow as create: guests -> date -> arrow-time)
        private void Update(ReservationModel updatedReservation)
        {
            Console.WriteLine($"{updatedReservation.GuestCount}");
            updatedReservation = _reservationsLogic.ReloadReservation(updatedReservation);
            Console.WriteLine($"{updatedReservation.GuestCount}");


            Console.Clear();
            Console.WriteLine("=== Update Reservation ===");
            // ===== Edit Reservation Menu =====
            string[] editOptions = {
            "Table Selection",
            "Guest Count",
            "Date & Time",
            "Dish Selection",
            "Back"
            };

            int editIndex = 0;
            bool editing = true;

            // Calculate the longest option name for alignment
            int longestLabel = editOptions.Max(opt => opt.Length) + 2;

            while (editing)
            {
                Console.Clear();
                Console.WriteLine("=== Update Reservation ===");

                for (int i = 0; i < editOptions.Length; i++)
                {
                    bool isSelected = i == editIndex;
                    if (isSelected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    // Current value formatting
                    string currentValue = i switch
                    {
                        0 => $"{updatedReservation.TableId}",
                        1 => $"{updatedReservation.GuestCount}",
                        2 => $"{DateTime.Parse(updatedReservation.StartAt):dd-MM-yyyy HH:mm}",
                        3 => $"{(_reservationsLogic.ReservationContainsDishes(updatedReservation) ? "Made" : "Not made yet")}",
                        _ => ""
                    };

                    // Align option name and current value neatly
                    string label = editOptions[i].PadRight(longestLabel);
                    Console.WriteLine($"  {label} {currentValue}");

                    if (isSelected)
                        Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine("↑/↓ to navigate, Enter to edit");

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        editIndex = (editIndex - 1 + editOptions.Length) % editOptions.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        editIndex = (editIndex + 1) % editOptions.Length;
                        break;

                    case ConsoleKey.Enter:
                        switch (editIndex)
                        {
                            case 0:
                                EditTableSelection(updatedReservation);
                                break;
                            case 1:
                                EditGuestCount(updatedReservation);
                                break;
                            case 2:
                                EditDateTime(updatedReservation);
                                break;
                            case 3:
                                EditDishSelection(updatedReservation);
                                break;
                            case 4: // User gets send back to selecting a reservation.
                                Start();
                                break;
                        }
                        break;
                }
            }
        }
        private void Delete(ReservationModel reservation)
        {
            _reservationsLogic.UpdateReservationStatus(reservation);
            if (_reservationsLogic.ReservationContainsDishes(reservation))
            {
                _dishLogic.DeleteDishesFromReservation(reservation);
            }
            Console.WriteLine();
            Console.WriteLine("Reservation Canceled");
            Thread.Sleep(1500);
            Start();
        }

        private void EditGuestCount(ReservationModel reservation)
        {
            Console.Clear();
            Console.WriteLine("\n=== Update Guestcount ===");
            Console.WriteLine($"\nCurrent guest count: {reservation.GuestCount}");

            int availableGuestCount = _reservationsLogic.GetAvailableGuestCount(reservation);

            if (availableGuestCount == 0)
            {
                Console.WriteLine("\nNo alternate guest count available for this reservation.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            string[] manageOptions = {
        $"Change guest count to {availableGuestCount}",
        "Back"
    };

            int manageIndex = 0;
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("\n=== Update Guestcount ===");
                Console.WriteLine($"\nCurrent guest count: {reservation.GuestCount}\n");

                // Menu rendering
                for (int i = 0; i < manageOptions.Length; i++)
                {
                    bool isSelected = i == manageIndex;
                    if (isSelected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine($"   {manageOptions[i]}");

                    if (isSelected)
                        Console.ResetColor();
                }

                // Handle key input
                var key = Console.ReadKey(true);

                switch (key.Key)
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
                            case 0: // User chooses to change guest count

                                // Perform dish selection and update logic
                                GuestCountDishSelection(reservation.GuestCount, availableGuestCount, reservation);


                                Console.Clear();
                                Console.WriteLine($"\nGuest count successfully updated to {availableGuestCount}!");
                                managing = false;
                                return;

                            case 1: // Back
                                managing = false;
                                break;
                        }
                        break;
                }
            }
        }


private void GuestCountDishSelection(int oldguestcount, int newguestcount, ReservationModel reservation, string inputstring = "Make a dish selection? (Y/N)")
{
    while (true)
    {
        Console.WriteLine($"{inputstring}");
        string? MakesDishSelection = Console.ReadLine()?.Trim().ToUpper();
        if (string.IsNullOrEmpty(MakesDishSelection))
        {
            Console.WriteLine("All fields are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            continue;
        }
        if (MakesDishSelection == "Y")
        {
            DishSelectionStep(newguestcount, oldguestcount, reservation);
            reservation.GuestCount = newguestcount;
            _reservationsLogic.UpdateGuestCountForReservation(reservation, newguestcount); // <-- hier
            break;
        }
        else if (MakesDishSelection == "N")
        {
            reservation.GuestCount = newguestcount;
            _reservationsLogic.UpdateGuestCountForReservation(reservation, newguestcount); // <-- hier
            if (oldguestcount > newguestcount)
            {
                _dishLogic.DeleteDishesFromReservation(reservation);
            }
            break;
        }
        else
        {
            continue;
        }
    }
}


        private void EditDishSelection(ReservationModel reservation)
        {
            bool ableToDelete = false;
            if (_reservationsLogic.ReservationContainsDishes(reservation))
            {
                Console.Clear();
                Console.WriteLine("\n=== Update Dish Selection ===");
                Console.WriteLine();
                Console.WriteLine("Dish selection already made");
                Console.WriteLine();
                ableToDelete = true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("\n=== Update Dish Selection ===");
                Console.WriteLine();
                Console.WriteLine("Dish selection not made yet");
                Console.WriteLine();
            }
            bool managing = true;
            int manageIndex = 0;

            string[] manageOptions = {
            "Change dish selection",
            "Remove dish selection",
            "Back"
            };

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("\n=== Edit Dish Selection ===");
                Console.WriteLine($"\nReservation for {reservation.GuestCount} guests\n");

                // Display menu options
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

                // Handle key input
                var key = Console.ReadKey(true);

                switch (key.Key)
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
                            case 0: // User makes the choice to change dish selection
                                DishSelectionStep(reservation.GuestCount, 0, reservation);
                                ReservationModel updated = _reservationsLogic.ReloadReservation(reservation);
                                Update(updated);
                                break;

                            case 1: // User makes the choice to remove his dishselections
                                if (ableToDelete)
                                {
                                    _dishLogic.DeleteDishesFromReservation(reservation);
                                    Console.WriteLine();
                                    Console.WriteLine("Deleted dish selection");
                                    Thread.Sleep(1500);
                                    ReservationModel updated2 = _reservationsLogic.ReloadReservation(reservation);
                                    Update(updated2);
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("Unable to delete a dish selection that doesn't exsist");
                                    Thread.Sleep(1500);
                                }
                                break;

                            case 2: // Back
                                Update(reservation);
                                managing = false;
                                break;
                        }
                        break;
                }
            }
        }

        private bool EditTableSelection(ReservationModel reservation)
        {
            
            TableAcces tableAccess = new TableAcces();
            List<TableModel> allTables = tableAccess.GetAllTables();
            List<int> reservedTableIds = tableAccess.GetNonAvailableOnDate(reservation.StartAt, reservation.GuestCount);

            // Show floor plan and let user select a table
            TableModel? AvailableTable = FloorPlanView.SelectTableFromFloorPlan(allTables, reservedTableIds, reservation.GuestCount);
            if (AvailableTable == null)
            {
                Console.WriteLine("Table selection cancelled.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return false;
            }
            if (AvailableTable.ID == reservation.TableId)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine($"Unable to change seating to table thats already selected.");
                Thread.Sleep(1500);
                return false;
            }
            reservation.TableId = AvailableTable.ID;
            _reservationsLogic.UpdateTableForReservation(reservation);
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"Changed seating to table {AvailableTable.ID}");
            Thread.Sleep(1500);
            return true;

        }

        private void DishSelectionStep(int newguestcount, int oldguestcount, ReservationModel reservation)
        {
            var dishLogic = new DishLogic();
            ThemeModel? correctTheme = dishLogic.GetCorrectTheme(reservation.StartAt);
            int askForDishesAmount = 0;
            if (_reservationsLogic.ReservationContainsDishes(reservation))
            {
                askForDishesAmount = newguestcount > oldguestcount ? newguestcount - oldguestcount : newguestcount;
            }
            else
            {
                askForDishesAmount = newguestcount;
            }
            if (correctTheme is not null)
            {
                // Show dish selection menu
                List<DishModel> selectedDishes = DishSelection.SelectDishesForReservation(askForDishesAmount, correctTheme.ID);

                if (selectedDishes.Count == 0)
                {
                    // User cancelled or something went wrong or didnt select any dishes.
                    ColorConsole.WriteWarning("Dish selection cancelled. Returning to Update Reservation...");
                    Thread.Sleep(1500);
                    return;
                }
                dishLogic.ReserveDishes(selectedDishes, reservation, newguestcount < oldguestcount);
                Thread.Sleep(2000);
            }
            else
            {
                ColorConsole.WriteWarning("No theme available for the chosen month. Proceeding without dish selection...");
                Thread.Sleep(2000);
            }
        }
        
        public void EditDateTime(ReservationModel reservation)
        {
                Console.Clear();
                Console.WriteLine("\n=== Update Date & Time ===");
                Console.WriteLine($"\nCurrent Date & Time: {DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}\n");

                string? ReservationDate = CalanderInput.Calander();

            Console.WriteLine("Select Arrival Time:");
            string ArrivalTime = _reservationsLogic.SelectArrivalTime();
            Console.WriteLine($"You selected: {ArrivalTime}");
            TimeSpan arrivalTime = TimeSpan.Parse(ArrivalTime);

            DateTime newStartAt = DateTime.ParseExact(ReservationDate, "dd-MM-yyyy", null) + arrivalTime;
            DateTime oldStartAt = DateTime.ParseExact(reservation.StartAt, "dd-MM-yyyy HH:mm", null);

            bool sameMonth = newStartAt.Month == oldStartAt.Month && newStartAt.Year == oldStartAt.Year;

            if (_reservationsLogic.ReservationContainsDishes(reservation))
            {
                if (!sameMonth)
                {
                    Console.WriteLine("\nNot the same month: removing dish selections...");
                    _dishLogic.DeleteDishesFromReservation(reservation);
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("\nThe same month keeping the dish selections...");
                    Thread.Sleep(2000);
                }
            }
            reservation.StartAt = newStartAt.ToString("dd-MM-yyyy HH:mm");
            reservation.TableId = 0;
            bool tablesuccess = false;

            // Loop until user selects a table successfully
            while (!tablesuccess)
            {
                tablesuccess = EditTableSelection(reservation);

                if (!tablesuccess)// if the table selection returns false the user gets shown this message and tries again.
                {
                    Console.Clear();
                    Console.WriteLine("No table selected. Please choose a table to continue.");
                    Thread.Sleep(1500);
                }
            }

            // Once a table is selected
            _reservationsLogic.UpdateDateTimeForReservation(reservation);
            Console.Clear();
            Console.WriteLine($"\nDate/time updated to {reservation.StartAt}");
            Thread.Sleep(1500);
        }
    }
}
