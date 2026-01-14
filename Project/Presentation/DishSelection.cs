using Project.DataModels;
using Project.DataAccess;
using Project.Logic;

public static class DishSelection
{
    
    private static DishAccess _dishAccess = new DishAccess();
    private static AllergenAccess _allergenAccess = new AllergenAccess();
    private static List<int> _activeAllergenFilters = new List<int>();

    
    public static List<DishModel> SelectDishesForReservation(int guestCount, int themeId)
    {
        ThemesLogic themeLogic = new ThemesLogic();
        ThemeModel theme = themeLogic.GetById(themeId);

        if (theme == null)
        {
            Console.WriteLine("Theme not found.");
            return new List<DishModel>();
        }

        return SelectDishesForReservation(guestCount, theme);
    }

    
    public static List<DishModel> SelectDishesForReservation(int guestCount, ThemeModel theme)
    {
        List<List<Drink>> selectedDrinksPerGuest = new List<List<Drink>>();
        ReservationsLogic reservationsLogic = new ReservationsLogic();

        List<List<DishModel?>> allSelectedDishesPerGuest = new List<List<DishModel?>>();
        List<Drink?> selectedDrinkPerGuest = new List<Drink?>();

        List<DishModel> allSelectedDishes = new List<DishModel>();
        var availableDishes = GetDishesByTheme(theme.ID);

        for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
        {
            _activeAllergenFilters.Clear();
            ManageAllergenFilters();

            List<DishModel> filteredDishes = ApplyAllergenFilters(availableDishes);

            var starters = filteredDishes.Where(d => d.Type == "Starter").ToList();
            var mains = filteredDishes.Where(d => d.Type == "Main").ToList();
            var desserts = filteredDishes.Where(d => d.Type == "Dessert").ToList();

            Console.Clear();
            DisplayThemeHeader(theme.Name, guestNumber, guestCount);

            bool isNotDishSelecting;
            List<DishModel?> guestDishes = new List<DishModel?>();

            var starter = SelectDish(starters, "Starters", guestNumber, out isNotDishSelecting);
            if (isNotDishSelecting) return new List<DishModel>();
            guestDishes.Add(starter);
            if (starter != null) allSelectedDishes.Add(starter);

            var main = SelectDish(mains, "Main Courses", guestNumber, out isNotDishSelecting);
            if (isNotDishSelecting) return new List<DishModel>();
            guestDishes.Add(main);
            if (main != null) allSelectedDishes.Add(main);

            Drink? drink = null;
            if (main != null)
                drink = SelectDrinkForMainDish(main.ID);

            selectedDrinkPerGuest.Add(drink);

            var dessert = SelectDish(desserts, "Desserts", guestNumber, out isNotDishSelecting);
            if (isNotDishSelecting) return new List<DishModel>();
            guestDishes.Add(dessert);
            if (dessert != null) allSelectedDishes.Add(dessert);

            allSelectedDishesPerGuest.Add(guestDishes);
        }

        if (ShowReservationSummary(allSelectedDishesPerGuest, selectedDrinkPerGuest, guestCount))
            return allSelectedDishes;

        return new List<DishModel>();
    }

    
    private static DishModel? SelectDish(List<DishModel> dishes, string courseType, int guestNumber, out bool IsNotDishSelecting)
    {
        IsNotDishSelecting = false;
        if (dishes.Count == 0)
        {
            ColorConsole.WriteError($"No {courseType} available!");
            Thread.Sleep(2000);
            return null;
        }
        
        int selectedIndex = 0;
        bool selecting = true;

        while (selecting)
        {
            Console.Clear();

            // Header
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  SELECT {courseType.ToUpper()} - Guest #{guestNumber}".PadRight(77) + "║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Use ↑↓ Arrow Keys to Navigate  |  Press ENTER to Select  |  ESC to Cancel ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();


            int totalOptions = dishes.Count + 1;
            for (int i = 0; i < totalOptions; i++)
            {
                bool isSelected = i == selectedIndex;

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (i < dishes.Count)
                {
                    var dish = dishes[i];
                    string dishLine = $"{i + 1}. {dish.Name.PadRight(30)} - €{dish.Price:F2}";
                    Console.WriteLine($"  {dishLine}");

                    if (isSelected)
                    {
                        Console.ResetColor();
                    }
                    Console.WriteLine();

                    if (isSelected)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"       {dish.Description}");
                        
                        if (dish.AllergenNames.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"       Contains: {string.Join(", ", dish.AllergenNames)}");
                        }
                        
                        Console.ResetColor();
                    }
                }
                else
                {
                    string noneOption = $"{dishes.Count + 1}. No Dish";
                    Console.WriteLine($"  {noneOption}");

                    if (isSelected)
                        Console.ResetColor();

                    Console.WriteLine();
                }
            }

            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + totalOptions) % totalOptions;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % totalOptions;
                    break;

                case ConsoleKey.Enter:
                    Console.WriteLine();
                    if (selectedIndex < dishes.Count)
                    {
                        var selected = dishes[selectedIndex];
                        Console.WriteLine($"Selected: {selected.Name}");
                        Thread.Sleep(1200);
                        return selected;
                    }
                    else
                    {
                        Console.WriteLine("No Dish Selected.");
                        Thread.Sleep(1200);
                        return null;
                    }
                case ConsoleKey.Escape:
                    IsNotDishSelecting = true;
                    return null;
            }
        }
        return null;
    }

    
    private static bool ShowReservationSummary(List<List<DishModel?>> allSelectedDishesPerGuest, List<Drink?> selectedDrinkPerGuest, int guestCount)
    {
        Console.Clear();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║    RESERVATION SUMMARY - PLEASE CONFIRM".PadRight(77) + "║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        decimal totalPrice = 0;

        for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
        {
            Console.WriteLine($"Guest #{guestNumber}:");

            var guestDishes = allSelectedDishesPerGuest[guestNumber - 1];
            Drink? drink = selectedDrinkPerGuest[guestNumber - 1];

            var starter = guestDishes.Count > 0 ? guestDishes[0] : null;
            var main    = guestDishes.Count > 1 ? guestDishes[1] : null;
            var dessert = guestDishes.Count > 2 ? guestDishes[2] : null;

            if (starter != null)
            {
                Console.WriteLine($"    Starter:  {starter.Name.PadRight(30)} €{starter.Price:F2}");
                totalPrice += starter.Price;
            }
            else
            {
                Console.WriteLine("    Starter:  Non Chosen");
            }

            if (main != null)
            {
                Console.WriteLine($"    Main:     {main.Name.PadRight(30)} €{main.Price:F2}");
                totalPrice += main.Price;
            }
            else
            {
                Console.WriteLine("    Main:     Non Chosen");
            }

            if (dessert != null)
            {
                Console.WriteLine($"    Dessert:  {dessert.Name.PadRight(30)} €{dessert.Price:F2}");
                totalPrice += dessert.Price;
            }
            else
            {
                Console.WriteLine("    Dessert:  Non Chosen");
            }

            if (drink != null)
            {
                Console.WriteLine($"    Drink:    {drink.Name.PadRight(30)} €{drink.Price:F2}");
                totalPrice += drink.Price;
            }
            else
            {
                Console.WriteLine("    Drink:    No Drink");
            }

            Console.WriteLine();
        }

        

        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine($"Total Price: €{totalPrice:F2}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.Write("Confirm this reservation? (Y/N): ");
        var response = Console.ReadLine()?.Trim().ToUpper();

        if (response == "Y")
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Your reservation with menu selection has been saved!");
            Console.WriteLine();
            Thread.Sleep(3000);
            return true;
        }

        return false;
    }


    
    private static List<DishModel> GetDishesByTheme(int themeId)
    {
        var dishIds = _dishAccess.GetallDishIdByThemeId(themeId);
        

        if (dishIds.Count > 0)
        {
            return _dishAccess.GetDishesByIds(dishIds);
        }
        
        return new List<DishModel>();
    }
    
    private static void DisplayThemeHeader(string themeName, int currentGuest, int totalGuests)
    {
        Console.WriteLine($"=== Theme: {themeName} - Guest {currentGuest} of {totalGuests} ===");
        Console.WriteLine();
    }

    private static List<DishModel> ApplyAllergenFilters(List<DishModel> dishes)
    {
        if (_activeAllergenFilters.Count == 0)
        {
            return dishes;
        }


        return dishes.Where(dish => 
            !dish.AllergenIds.Any(allergenId => _activeAllergenFilters.Contains(allergenId))
        ).ToList();
    }

    private static void ManageAllergenFilters()
{
    List<AllergenModel> allergens = _allergenAccess.GetAll();
    List<bool> selectedStates = new List<bool>();
    
    for (int i = 0; i < allergens.Count; i++)
    {
        selectedStates.Add(_activeAllergenFilters.Contains(allergens[i].ID));
    }

    int currentIndex = 0;
    bool selecting = true;

    while (selecting)
    {
        Console.Clear();
        Console.WriteLine("=== Select allergens to avoid ===\n");

        for (int i = 0; i < allergens.Count; i++)
        {
            bool isSelected = i == currentIndex;
            bool isChecked = selectedStates[i];

            if (isSelected)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
            }

            string checkbox = isChecked ? "[X]" : "[ ]";
            Console.WriteLine($"{checkbox} {allergens[i].Name}");

            if (isSelected)
            {
                Console.ResetColor();
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press SPACE to select | ENTER to continue");

        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                currentIndex = (currentIndex - 1 + allergens.Count) % allergens.Count;
                break;

            case ConsoleKey.DownArrow:
                currentIndex = (currentIndex + 1) % allergens.Count;
                break;

            case ConsoleKey.Spacebar:
                selectedStates[currentIndex] = !selectedStates[currentIndex];
                break;

            case ConsoleKey.Enter:
                _activeAllergenFilters.Clear();
                for (int i = 0; i < allergens.Count; i++)
                {
                    if (selectedStates[i])
                        _activeAllergenFilters.Add(allergens[i].ID);
                }
                selecting = false;
                break;
        }
    }
}


    private static Drink? ShowDrinkForMainDish(int dishId)
    {
        DrinkLogic drinkLogic = new DrinkLogic();
        Drink drink = drinkLogic.GetDrinkForDish(dishId);

        if (drink == null)
        {
            Console.WriteLine("No drink is linked to this dish.");
            Thread.Sleep(1200);
            return null;
        }

        Console.WriteLine();
        Console.WriteLine("This main dish comes with:");
        Console.WriteLine($"{drink.Name} ({drink.AlcoholPercentage}%) - €{drink.Price:F2}");
        Console.Write("Do you want this drink? (y/n): ");

        string choice = Console.ReadLine();
        choice = choice == null ? "" : choice.ToLower();

        if (choice == "y")
        {
            Console.WriteLine("Drink added.");
            Thread.Sleep(1000);
            return drink;
        }

        Console.WriteLine("Drink skipped.");
        Thread.Sleep(1000);
        return null;
    }

    private static Drink? SelectDrinkForMainDish(int dishId)
{
    DrinkLogic drinkLogic = new DrinkLogic();
    DishLogic dishLogic = new DishLogic();

    List<Drink> drinks = drinkLogic.GetAllDrinks();
    if (drinks.Count == 0)
        return null;

    Drink? recommended = drinkLogic.GetDrinkForDish(dishId);

    int index = 0;

    if (recommended != null)
    {
        index = drinks.FindIndex(d => d.ID == recommended.ID);
    }


    DishModel dish = dishLogic.GetById(dishId);
    string dishName = dish?.Name ?? "this dish";

    

    while (true)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine(
            $"║ Our wine steward recommends this drink for your dish       ║"
        );
        Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║ ↑↓ Navigate | ENTER Select | ESC Skip                      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        for (int i = 0; i <= drinks.Count; i++)
        {
            bool selected = i == index;

            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (i < drinks.Count)
            {
                Drink d = drinks[i];
                bool isRecommended = recommended != null && d.ID == recommended.ID;
                string marker = isRecommended ? " ★ recommended" : "";

                Console.WriteLine(
                    $"  {d.Name.PadRight(25)} €{d.Price:F2}{marker}"
                );
            }
            else
            {
                Console.WriteLine("  No drink");
            }

            if (selected)
                Console.ResetColor();
        }

        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                index = (index - 1 + drinks.Count + 1) % (drinks.Count + 1);
                break;

            case ConsoleKey.DownArrow:
                index = (index + 1) % (drinks.Count + 1);
                break;

            case ConsoleKey.Enter:
                return index < drinks.Count ? drinks[index] : null;

            case ConsoleKey.Escape:
                return null;
        }
    }
}

}
