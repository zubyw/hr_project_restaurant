using Project.DataModels;

public static class DishSelection
{
    private static DishAccess _dishAccess = new DishAccess();
    
    public static List<DishModel> SelectDishesForReservation(int guestCount, int themeId)
    {
        List<DishModel> allSelectedDishes = new List<DishModel>();
        
        // Get all available dishes for this theme
        var availableDishes = GetDishesByTheme(themeId);
        
        // Separate by type
        var starters = availableDishes.Where(d => d.Type == "Starter").ToList();
        var mains = availableDishes.Where(d => d.Type == "Main").ToList();
        var desserts = availableDishes.Where(d => d.Type == "Dessert").ToList();
        
        // Get theme name for display
        string themeName = GetThemeName(themeId);
        
        // Each guest selects dishes
        List<List<DishModel?>> allSelectedDishesPerGuest = new List<List<DishModel?>>();

        for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
        {
            Console.Clear();
            DisplayThemeHeader(themeName, guestNumber, guestCount);

            bool IsNotDishSelecting;

            List<DishModel?> guestDishes = new List<DishModel?>();

            // Select starter
            var selectedStarter = SelectDish(starters, "Starters", guestNumber, out IsNotDishSelecting);
            if (IsNotDishSelecting) return new List<DishModel>(); // User cancelled
            guestDishes.Add(selectedStarter); // can be null
            if (selectedStarter != null) allSelectedDishes.Add(selectedStarter);

            // Select main
            var selectedMain = SelectDish(mains, "Main Courses", guestNumber, out IsNotDishSelecting);
            if (IsNotDishSelecting) return new List<DishModel>(); // User cancelled
            guestDishes.Add(selectedMain);
            if (selectedMain != null) allSelectedDishes.Add(selectedMain);

            // Select dessert
            var selectedDessert = SelectDish(desserts, "Desserts", guestNumber, out IsNotDishSelecting);
            if (IsNotDishSelecting) return new List<DishModel>(); // User cancelled
            guestDishes.Add(selectedDessert);
            if (selectedDessert != null) allSelectedDishes.Add(selectedDessert);
            

            allSelectedDishesPerGuest.Add(guestDishes);
        }
    

        
        // Show summary and confirm
        if (ShowReservationSummary(allSelectedDishesPerGuest, guestCount))
        {
            return allSelectedDishes;
        }
        
        return new List<DishModel>(); // User didn't confirm
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

            // Display dishes
            int totalOptions = dishes.Count + 1;
            for (int i = 0; i < totalOptions; i++)
            {
                bool isSelected = i == selectedIndex;

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ► ");
                }
                else
                {
                    Console.Write("    ");
                }
                if (i < dishes.Count)
                {
                    var dish = dishes[i];
                    string dishLine = $"{i + 1}. {dish.Name.PadRight(30)} - €{dish.Price:F2}";
                    Console.Write(dishLine);

                    if (isSelected)
                    {
                        Console.ResetColor();
                    }
                    Console.WriteLine();

                    // Show description for selected item
                    if (isSelected)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"       {dish.Description}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    string noneOption = "5. No Dish";
                    Console.Write(noneOption);

                    if (isSelected)
                        Console.ResetColor();

                    Console.WriteLine();
                }
            }

            // Handle input
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
                    // Confirm selection of dish
                    if (selectedIndex < dishes.Count)
                    {
                        var selected = dishes[selectedIndex];
                        Console.WriteLine($"✓ Selected: {selected.Name}");
                        Thread.Sleep(1200);
                        return selected;
                    }
                    // Confirm selection of not choosing a dish
                    else
                    {
                        Console.WriteLine("✓ No Dish Selected.");
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
    

    // Group by guest
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
        Console.WriteLine("✅ Your reservation with menu selection has been saved!");
        Console.WriteLine();
        Thread.Sleep(3000);
        return true;
    }

    return false;
}

    
    private static List<DishModel> GetDishesByTheme(int themeId)
    {
        // Get all dish IDs for this theme
        var dishIds = _dishAccess.GetallDishIdByThemeId(themeId);
        
        // Get the actual dishes
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
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        ColorConsole.WriteTitle($"║  🍣 Theme of the Month: {themeName}".PadRight(77) + "║");
        ColorConsole.WriteInfo($"║  Selecting dishes for Guest {currentGuest} of {totalGuests}".PadRight(77) + "║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
    }
}