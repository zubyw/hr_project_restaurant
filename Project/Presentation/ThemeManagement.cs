using System;
using System.Collections.Generic;
using Project.Presentation;
using Project.Logic;
public static class ThemeManagement
{
    private static readonly ThemesLogic logic = new ThemesLogic();

    public static void Start()
    {
        string[] options =
        {
            "Create new theme",
            "Manage all themes",
            "Manage theme calander",
            "Back"
        };

        while (true)
        {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Themes Management ===");
        try
        {
            switch (index)
            {
                case 0:
                    Add();
                    break;

                case 1:
                    ManageTheme();
                    break;

                case 2:
                    ManageThemeCalendar();
                    break;
                case 3:
                    Menu.ShowAdminMenu();
                    break;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
        }
    }
    }

    private static void Add()
    {
        Console.Clear();
        Console.WriteLine("=== Create new theme ===");
        
        Console.Write("Name: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Name cannot be empty.");
        Add();
        }
        if(logic.DoesThemeExist(name))
        {
            Console.WriteLine("There already is a theme with this name");
            Thread.Sleep(1500);
            Start();
        }
        Console.Clear();
        Console.WriteLine("=== Create new theme ===");
        Console.Write("Description");
        string? description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Description cannot be empty.");
        Add();
        }

        ThemeModel theme = new ThemeModel(){Name = name, Course = description};
        logic.WriteTheme(theme);
        Start();
    }


    private static ThemeModel? ManageAllThemes()
    {
        Console.Clear();
        List<ThemeModel> themes = logic.GetAll();

        if (themes.Count == 0)
        {
            Console.WriteLine("No themes found.");
            return null;
        }
        int index = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("All themes");
            Console.WriteLine("┌────────────────────────────┬──────────────────────────────────────────────────────────────┐");
            Console.WriteLine("│ Name                       │ Description                                                  │");
            Console.WriteLine("├────────────────────────────┼──────────────────────────────────────────────────────────────┤");

            for (int i = 0; i < themes.Count; i++)
            {
                var theme = themes[i];

                string name = theme.Name.Length > 26 ? theme.Name[..23] + "..." : theme.Name;
                string description = theme.Course.Length > 60 ? theme.Course[..57] + "..." : theme.Course;

                bool selected = (i == index);

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine("│ {0,-26} │ {1,-60} │", name, description);

                if (selected)
                    Console.ResetColor();
            }

            Console.WriteLine("└────────────────────────────┴──────────────────────────────────────────────────────────────┘");


            Console.WriteLine("\n ↑↓ Select theme | ESC = back");

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow)
            {
                index = (index + 1) % themes.Count;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                index = (index - 1 + themes.Count) % themes.Count;
            }
            else if (key == ConsoleKey.Enter)
            {
                ThemeModel selectedTheme = themes[index];
                return selectedTheme;
            }
            else if (key == ConsoleKey.Escape)
            {
                Start();
            }
        }
    }


    private static void ManageTheme()
    {
        ThemeModel? theme = ManageAllThemes();
        if (theme is null)
        {
            Start();
        }
        string[] options =
        {
            "Edit theme",
            "Manage dishes in theme",
            "Delete theme",
            "Back"
        };

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Manage theme ===");
        try
        {
            switch (index)
            {
                case 0:
                    EditTheme(theme);
                    break;

                case 1:
                    DishesInTheme(theme);
                    break;

                case 2:
                    Delete(theme);
                    break;
                case 3:
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
        }
    }
    }

    private static void EditTheme(ThemeModel theme)
    {
        string[] options =
        {
            "Edit Name",
            "Edit Description",
            "Back"
        };

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage theme ===\n\n{theme.ToString()}");
        try
        {
            switch (index)
            {
                case 0:
                    Console.Clear();
                    Console.WriteLine($"Name: {theme.Name}");
                    Console.WriteLine();
                    string? newname = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newname))
                    {
                    Console.WriteLine("Name cannot be empty.");
                    Thread.Sleep(1500);
                    continue;
                    }
                    theme.Name = newname;
                    logic.UpdateTheme(theme);
                    return;

                case 1:
                    Console.Clear();
                    Console.WriteLine($"Description: {theme.Course}");
                    Console.WriteLine();
                    string? newcourse = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newcourse))
                    {
                    Console.WriteLine("Name cannot be empty.");
                    Thread.Sleep(1500);
                    return;
                    }
                    theme.Course = newcourse;
                    logic.UpdateTheme(theme);
                    break;

                case 2:
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
        }
    }
    }

    private static void DishesInTheme(ThemeModel theme)
    {
        string[] options =
        {
            "Add dishes",
            "Delete dish",
            "Back"
        };

        while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage dishes in theme ===");
            try
            {
                switch (index)
                {
                    case 0:
                        AddDishesToTheme(theme);
                        return;

                    case 1:
                        DeleteDishesFromTheme(theme);
                        return;

                    case 2:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }
        }
    }

    private static DishModel? ChooseDishes(List<DishModel> dishes)
    {
        bool choice = false;
        int selected = 0;
        List<string> hidden = [];
        DisplayDishes(dishes, selected, hidden);
        List<DishModel> visible = logic.hidefilter(dishes, hidden);
        while (!choice)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Enter) {
            choice = true;
        }
        else
        {
                if (key == ConsoleKey.DownArrow)
                    {
                        selected = (selected + 1) % visible.Count;
                    }
                    else if (key == ConsoleKey.UpArrow)
                    {
                        selected = (selected - 1 + visible.Count) % visible.Count;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        return null;
                    }
                    else if (key == ConsoleKey.D || key == ConsoleKey.S || key == ConsoleKey.M || key == ConsoleKey.T){
                        selected = 0;
                        string input = key.ToString().ToLower();
                        if (hidden.Contains(input)) hidden.Remove(input);
                        else hidden.Add(input);
                        visible = logic.hidefilter(dishes, hidden);
                    }
                DisplayDishes(visible, selected, hidden);
            }
        
        }
        List<DishModel> endlist = logic.hidefilter(dishes, hidden);
        DishModel selectedDish = endlist[selected];
        return selectedDish;
    }

    private static void DisplayDishes(List<DishModel> dishes, int index, List<string> hidden)
    {
        Console.Clear();
        Console.WriteLine();

        // Header
        Console.WriteLine("┌────────────────────────┬───────────────┬───────────┬───────────────────────────────┐");
        Console.WriteLine("│ Name                   │ Type          │ Price     │ Linked to Themes              │");
        Console.WriteLine("├────────────────────────┼───────────────┼───────────┼───────────────────────────────┤");

        // Cache all themes for each dish to avoid repeated DB queries
        var dishThemes = dishes.ToDictionary(
            d => d.ID,
            d => logic.themesLinkedToDish(d) // your method
        );

        for (int i = 0; i < dishes.Count; i++)
        {
            var dish = dishes[i];

            string name = dish.Name.Length > 20 ? dish.Name[..17] + "..." : dish.Name;
            string type = dish.Type.Length > 12 ? dish.Type[..9] + "..." : dish.Type;

            var themes = dishThemes[dish.ID];
            string themeNames = string.Join(", ", themes.Select(t => t.Name));

            if (themeNames.Length > 27) themeNames = themeNames[..24] + "...";

            bool selected = (i == index);

            // Highlight selected row
            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("│ {0,-22} │ {1,-13} │ {2,9:F2} │ {3,-29} │",
                name, type, dish.Price, themeNames);

            if (selected)
                Console.ResetColor();
        }

        
        Console.WriteLine("└────────────────────────┴───────────────┴───────────┴───────────────────────────────┘");

        Console.WriteLine("\n ↑↓ Select dish | ESC = done/back");
        Console.WriteLine("Press D, S, M, or T to hide/show Desserts, Starters, Mains, or dishes already linked to a theme.");
        Console.WriteLine(logic.DisplayHiddenStatus(hidden));
    }

    private static void AddDishesToTheme(ThemeModel theme)
    {
        List<DishModel> dishes = logic.GetAllAvailableDishes(theme);
        List<DishModel> chosenDishes = [];

        if (dishes.Count == 0)
        {
            Console.WriteLine("No dishes found.");
            Console.ReadKey();
            return;
        }
        bool choosingdishes = true;
        while (choosingdishes)
        {
            DishModel chosendish = ChooseDishes(dishes);
            if (chosendish is null) choosingdishes = false;
            else
            {
                dishes.Remove(chosendish);
                chosenDishes.Add(chosendish);
            }
        }
        if (chosenDishes.Count() == 0)
        {
            Start();
        }
        logic.AddDishesToTheme(chosenDishes, theme);
        Console.WriteLine("Added dishes to theme ");
        Thread.Sleep(1500);
        return; 
    }

    private static void DeleteDishesFromTheme(ThemeModel theme)
    {
        List<DishModel> allDishesInTheme = logic.GetAllDishesInTheme(theme);

        DishModel chosendish = ChooseDishes(allDishesInTheme);
        if (chosendish is null)
        {
            Console.Clear();
            Console.WriteLine("Back to Menu");
            Thread.Sleep(1500);
            Start();
        }
        else
        {
            logic.DeleteDishonTheme(chosendish, theme);
            Console.Clear();
            Console.WriteLine("Deleted dish on theme");
            Thread.Sleep(1500);
            Start();
        }

    }


    private static void Delete(ThemeModel theme)
    {
        string[] options =
            {
                "Yes",
                "No"
            };

            while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: delete theme ===\n\n{theme.ToString()}\n\nDelete Theme?");
            try
            {
                switch (index)
                {
                    case 0:
                        logic.DeleteThemeCompletely(theme);
                        Console.WriteLine($"Theme: {theme.Name} deleted");
                        Thread.Sleep(1500);
                        ManageAllThemes();
                        return;
                    case 1:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }
        }
    }

    private static void ManageThemeCalendar()
    {
        string[] options =
            {
                "View theme calendar",
                "Set month/s to theme",
                "Back"
            };

            while (true)
            {
                int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage theme calendar ===");
                try
                {
                    switch (index)
                    {
                        case 0:
                            ViewMenu.Start();
                            return;
                        case 1:
                            LinkMonthsToTheme();
                            return;
                        case 2:
                            Start();
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ReadKey();
                }
            }


        
    }

    private static void LinkMonthsToTheme()
    {
        ThemeModel? selectedtheme = ManageAllThemes();
        if (selectedtheme is null) Start();

        //this gets all the available months (in logic it checks which months are already chosen)
        List<string> monthOptions = logic.GetAvailableMonths();


        monthOptions.Add("Done");

        List<string> chosenMonths = new List<string>();

        while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(monthOptions.ToArray(), "=== Admin: Choose months ===");

            string selected = monthOptions[index];

            if (selected == "Done")
                break;

            if (chosenMonths.Contains(selected))
            {
                Console.Clear();
                Console.WriteLine($"This month already is in this selection: {selected}");
                Thread.Sleep(1500);
                continue;
            }

            // Add month to the chosen list
            Console.Clear();
            Console.WriteLine($"Added: {selected}");

            chosenMonths.Add(DateTime.ParseExact(selected, "MM-yyyy", null).ToString("yyyy-MM"));

            Thread.Sleep(1500);
        }

        Console.Clear();
        Console.WriteLine("Saved final month selection");
        Console.WriteLine("Final month selection:");
        foreach (var m in chosenMonths)
        {
            Console.WriteLine(" - " + m);
        }
        
        Thread.Sleep(3000);
        logic.LinkMonthsToTheme(chosenMonths, selectedtheme);
        Thread.Sleep(3000);
        ManageThemeCalendar();
    }
}
