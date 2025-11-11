using System;
using System.Collections.Generic;
using System.Globalization; // <-- toegevoegd
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
        public void Start(int userId)
        {
            // needed for ownership checks in logic
            ReservationsLogic.CurrentUserId = userId;

            Console.Clear();
            Console.WriteLine("=== My Reservations ===");
            Console.WriteLine();

            // Load all reservations for the logged-in user
            List<ReservationModel> reservations = _reservationsLogic.GetReservationsByUserIdForGuest(userId);

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
                    string dateTime = DateTime.Parse(r.StartAt).ToString("dd-MM-yyyy HH:mm");
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
                        return; // go back or exit
                }
            }

            // After user presses Enter:
            ReservationModel selectedReservation = reservations[selectedIndex];

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
                                break;

                            case 1: // Cancel
                                Delete(selectedReservation, userId);
                                break;

                            case 2: // Back
                                managing = false;
                                Start(userId);
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
        private void Update(ReservationModel reservation)
        {
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
                        0 => $"{reservation.TableId}",
                        1 => $"{reservation.GuestCount}",
                        2 => $"{DateTime.Parse(reservation.StartAt):dd-MM-yyyy HH:mm}",
                        3 => $"{(_reservationsLogic.ReservationContainsDishes(reservation) ? "Not made yet" : "Made")}",
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
                                // EditTable(selectedReservation);
                                break;
                            case 1:
                                EditGuestCount(reservation);
                                break;
                            case 2:
                                // EditDateTime(selectedReservation);
                                break;
                            case 3:
                                // EditDish(selectedReservation);
                                break;
                            case 4:
                                editing = false; // Back
                                break;
                        }
                        break;
                }
            }
        }
        private void Delete(ReservationModel reservation, int userId)
        {
            return;
        }

        private void EditGuestCount(ReservationModel reservation)
        {
            Console.Clear();
            Console.WriteLine("\n===Reservations===");
            Console.WriteLine();
            Console.WriteLine($"Amount of people: {reservation.GuestCount}");
            Console.WriteLine();
            Console.WriteLine("Enter new amount of people: (1-6)");
            string? AmountPeople = Console.ReadLine();
            if (string.IsNullOrEmpty(AmountPeople))
            {
                Console.WriteLine("All fields are required!");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                EditGuestCount(reservation);
                return;
            }
            if (!UserMakeReservationLogic.CheckAmountPeople(AmountPeople))
            {
                Console.WriteLine("Given amount of people incorrect (1-6)");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                EditGuestCount(reservation);
                return;
            }
            if (int.TryParse(AmountPeople, out int intAmountPeople))
            {
                if (intAmountPeople == reservation.GuestCount)
                {
                    Console.WriteLine("Same guest count given");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    EditGuestCount(reservation);
                    return;
                }
            }
                GuestCountDishSelection(reservation.GuestCount, intAmountPeople, reservation);
        }

        private void GuestCountDishSelection(int oldguestcount, int newguestcount, ReservationModel reservation)
        {
            Console.WriteLine("Make a dish selection? (Y/N)");
            string? MakesDishSelection = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrEmpty(MakesDishSelection))
            {
                Console.WriteLine("All fields are required!");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
                GuestCountDishSelection(oldguestcount, newguestcount, reservation);
                return;
            }
            if (MakesDishSelection == "Y")
            {
                // ===== DISH SELECTION STEP =====
                // Get current theme
                var dishLogic = new DishLogic();
                int? currentThemeId = dishLogic.GetCurrentThemeId();
                int askForDishesAmount = 0;
                if (_reservationsLogic.ReservationContainsDishes(reservation))
                {
                    askForDishesAmount = newguestcount > oldguestcount ? newguestcount - oldguestcount : newguestcount;
                }
                else
                {
                    askForDishesAmount = newguestcount;
                }
                if (currentThemeId.HasValue)
                {
                    // Show dish selection menu
                    List<DishModel> selectedDishes = DishSelection.SelectDishesForReservation(askForDishesAmount, currentThemeId.Value);

                    if (selectedDishes.Count == 0)
                    {
                        // User cancelled or something went wrong or didnt select any dishes.
                        ColorConsole.WriteWarning("Dish selection cancelled. Returning to main menu...");
                        Thread.Sleep(1500);
                        GuestCountDishSelection(oldguestcount, newguestcount, reservation);
                        return;
                    }

                    _reservationsLogic.UpdateGuestCountForReservation(newguestcount, reservation);
                    dishLogic.ReserveDishes(selectedDishes, reservation, newguestcount < oldguestcount);
                    ReservationModel updatedreservation = reservation;
                    updatedreservation.GuestCount = newguestcount;
                    Update(updatedreservation);
                }
                else
                {
                    ColorConsole.WriteWarning("No theme available for the current month. Proceeding without dish selection...");
                    Thread.Sleep(2000);
                }
            }
            else if (MakesDishSelection == "N")
            {
                DishLogic dishLogic = new DishLogic();
                _reservationsLogic.UpdateGuestCountForReservation(newguestcount, reservation);
                if (oldguestcount > newguestcount)
                {
                    _dishLogic.DeleteDishesFromReservation(reservation);
                }
                ReservationModel updatedreservation = reservation;
                updatedreservation.GuestCount = newguestcount;
                Update(updatedreservation);
            }
            else
            {
                GuestCountDishSelection(oldguestcount, newguestcount, reservation);
            }
        }
    }
}
