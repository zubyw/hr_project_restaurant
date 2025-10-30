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
        for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
        {
            Console.Clear();
            DisplayThemeHeader(themeName, guestNumber, guestCount);
            
            // Select starter
            var selectedStarter = SelectDish(starters, "Starters", guestNumber);
            if (selectedStarter == null) return new List<DishModel>(); // User cancelled
            allSelectedDishes.Add(selectedStarter);
            
            // Select main
            var selectedMain = SelectDish(mains, "Main Courses", guestNumber);
            if (selectedMain == null) return new List<DishModel>(); // User cancelled
            allSelectedDishes.Add(selectedMain);
            
            // Select dessert
            var selectedDessert = SelectDish(desserts, "Desserts", guestNumber);
            if (selectedDessert == null) return new List<DishModel>(); // User cancelled
            allSelectedDishes.Add(selectedDessert);
        }
        
        // Show summary and confirm
        if (ShowReservationSummary(allSelectedDishes, guestCount))
        {
            return allSelectedDishes;
        }
        
        return new List<DishModel>(); // User didn't confirm
    }
    
    private static DishModel? SelectDish(List<DishModel> dishes, string courseType, int guestNumber)
    {
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
            ColorConsole.WriteTitle($"║  SELECT {courseType.ToUpper()} - Guest #{guestNumber}".PadRight(77) + "║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Use ↑↓ Arrow Keys to Navigate  |  Press ENTER to Select  |  ESC to Cancel ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            
            // Display dishes
            for (int i = 0; i < dishes.Count; i++)
            {
                var dish = dishes[i];
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
            
            // Handle input
            var key = Console.ReadKey(true);
            
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + dishes.Count) % dishes.Count;
                    break;
                    
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % dishes.Count;
                    break;
                    
                case ConsoleKey.Enter:
                    // Confirm selection
                    var selected = dishes[selectedIndex];
                    Console.WriteLine();
                    ColorConsole.WriteSuccess($"✓ Selected: {selected.Name}");
                    Thread.Sleep(800);
                    return selected;
                    
                case ConsoleKey.Escape:
                    return null;
            }
        }
        
        return null;
    }
    
    private static bool ShowReservationSummary(List<DishModel> selectedDishes, int guestCount)
    {
        Console.Clear();
        
        // Header
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        ColorConsole.WriteTitle("║    RESERVATION SUMMARY - PLEASE CONFIRM".PadRight(77) + "║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        decimal totalPrice = 0;
        
        // Group by guest
        for (int guestNumber = 1; guestNumber <= guestCount; guestNumber++)
        {
            int startIndex = (guestNumber - 1) * 3;
            
            ColorConsole.WriteHighlight($"Guest #{guestNumber}:");
            Console.WriteLine($"    Starter:  {selectedDishes[startIndex].Name.PadRight(30)} €{selectedDishes[startIndex].Price:F2}");
            Console.WriteLine($"    Main:     {selectedDishes[startIndex + 1].Name.PadRight(30)} €{selectedDishes[startIndex + 1].Price:F2}");
            Console.WriteLine($"    Dessert:  {selectedDishes[startIndex + 2].Name.PadRight(30)} €{selectedDishes[startIndex + 2].Price:F2}");
            Console.WriteLine();
            
            totalPrice += selectedDishes[startIndex].Price + 
                         selectedDishes[startIndex + 1].Price + 
                         selectedDishes[startIndex + 2].Price;
        }
        
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        ColorConsole.WriteSuccess($"Total Price: €{totalPrice:F2}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine();
        
        Console.Write("Confirm this reservation? (Y/N): ");
        var response = Console.ReadLine()?.Trim().ToUpper();
        
        if (response == "Y")
        {
            Console.Clear();
            Console.WriteLine();
            ColorConsole.WriteSuccess("✅ Your reservation with menu selection has been saved!");
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