using Project.DataModels;
using Project.DataAccess;

public static class DishSelection
{
    private static DishAccess _dishAccess = new DishAccess();
    private static AllergenAccess _allergenAccess = new AllergenAccess();
    private static List<int> _activeAllergenFilters = new List<int>();
    
    public static List<DishModel> SelectDishesForReservation(int guestCount, int themeId)
    {
        _activeAllergenFilters.Clear();
        
        ManageAllergenFilters();
        
        List<DishModel> allSelectedDishes = new List<DishModel>();
        
        var availableDishes = GetDishesByTheme(themeId);
        availableDishes = ApplyAllergenFilters(availableDishes);
        

        var starters = availableDishes.Where(d => d.Type == "Starter").ToList();
        var mains = availableDishes.Where(d => d.Type == "Main").ToList();
        var desserts = availableDishes.Where(d => d.Type == "Dessert").ToList();
        
        string themeName = GetThemeName(themeId);
        

        List<List<DishModel?>> allSelectedDishesPerGuest = new List<List<DishModel?>>();

        for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
        {
            Console.Clear();
            DisplayThemeHeader(themeName, guestNumber, guestCount);

            bool IsNotDishSelecting;

            List<DishModel?> guestDishes = new List<DishModel?>();

            var selectedStarter = SelectDish(starters, "Starters", guestNumber, out IsNotDishSelecting);
            if (IsNotDishSelecting) return new List<DishModel>();
            guestDishes.Add(selectedStarter);
            if (selectedStarter != null) allSelectedDishes.Add(selectedStarter);

            var selectedMain = SelectDish(mains, "Main Courses", guestNumber, out IsNotDishSelecting);
            if (IsNotDishSelecting) return new List<DishModel>();
            guestDishes.Add(selectedMain);
            if (selectedMain != null) allSelectedDishes.Add(selectedMain);

            var selectedDessert = SelectDish(desserts, "Desserts", guestNumber, out IsNotDishSelecting);
            if (IsNotDishSelecting) return new List<DishModel>();
            guestDishes.Add(selectedDessert);
            if (selectedDessert != null) allSelectedDishes.Add(selectedDessert);
            

            allSelectedDishesPerGuest.Add(guestDishes);
        }
    

        
        if (ShowReservationSummary(allSelectedDishesPerGuest, guestCount))
        {
            return allSelectedDishes;
        }
        
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
                    string noneOption = "5. No Dish";
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

    
    private static bool ShowReservationSummary(List<List<DishModel?>> allSelectedDishesPerGuest, int guestCount)
{
    Console.Clear();
    
    // Header
    Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║    RESERVATION SUMMARY - PLEASE CONFIRM".PadRight(77) + "║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
    Console.WriteLine();

    decimal totalPrice = 0;
    

    for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
    {

        Console.WriteLine($"Guest #{guestNumber}:");

        var guestDishes = allSelectedDishesPerGuest[guestNumber - 1];

        var starter = guestDishes.Count > 0 ? guestDishes[0] : null;
        var main    = guestDishes.Count > 1 ? guestDishes[1] : null;
        var dessert = guestDishes.Count > 2 ? guestDishes[2] : null;

        Console.WriteLine($"    Starter:  {(starter != null ? starter.Name.PadRight(30) + $" €{starter.Price:F2}" : "Non Chosen")}");
        Console.WriteLine($"    Main:     {(main != null ? main.Name.PadRight(30) + $" €{main.Price:F2}" : "Non Chosen")}");
        Console.WriteLine($"    Dessert:  {(dessert != null ? dessert.Name.PadRight(30) + $" €{dessert.Price:F2}" : "Non Chosen")}");
        Console.WriteLine();

        if (starter != null) totalPrice += starter.Price;
        if (main != null) totalPrice += main.Price;
        if (dessert != null) totalPrice += dessert.Price;
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
    
    private static string GetThemeName(int themeId)
    {
        var themeAccess = new ThemeAccess();
        var theme = themeAccess.GetById(themeId);
        return theme?.Name ?? "Special Menu";
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

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Select allergens to avoid ===");
            Console.WriteLine();

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
            Console.WriteLine("Press SPACE to toggle | ENTER to continue");

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
                        {
                            _activeAllergenFilters.Add(allergens[i].ID);
                        }
                    }
                    return;
            }
        }
    }
}
