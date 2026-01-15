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
            "Manage theme calendar",
            "Back"
        };
        bool choosing = true;
        while (choosing)
        {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Themes Management ===");
        
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
                    choosing = false;
                    break;

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
            Console.ReadKey();
            return;
        }

        if (logic.DoesThemeExist(name))
        {
            Console.WriteLine("There already is a theme with this name.");
            Thread.Sleep(1500);
            return;
        }

        Console.Clear();
        Console.WriteLine("=== Create new theme ===");

        Console.Write("Description: ");
        string? description = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description cannot be empty.");
            Console.ReadKey();
            return;
        }

        ThemeModel theme = new ThemeModel
        {
            Name = name,
            Course = description
        };

        logic.WriteTheme(theme);
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
        ThemeModel selectedTheme = null;
        int index = 0;
        bool pickingTheme = true;
        while (pickingTheme)
        {
            Console.Clear();
            Console.WriteLine("All themes");
            Console.WriteLine("┌────────────────────────────┬──────────────────────────────────────────────────────────────┐");
            Console.WriteLine("│ Name                       │ Description                                                  │");
            Console.WriteLine("├────────────────────────────┼──────────────────────────────────────────────────────────────┤");

            for (int i = 0; i < themes.Count; i++)
            {
                ThemeModel theme = themes[i];

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
                selectedTheme = themes[index];
                pickingTheme = false;
            }
            else if (key == ConsoleKey.Escape)
            {
                pickingTheme = false;
                selectedTheme = null;
            }
        }
        return selectedTheme;
    }


    private static void ManageTheme()
    {
        ThemeModel? theme = ManageAllThemes();
        if (theme is null)
        {
            // user pressed esc in ManageAllThemes
            return;
        }
        string[] options =
        {
            "Edit theme",
            "Manage dishes in theme",
            "Delete theme",
            "Back"
        };
        bool choosing = true;
        while (choosing)
    {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Manage theme ===");
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
                choosing = false;
                break;

            case 3:
                choosing = false;
                break;
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

        bool editing = true;
        while (editing)
        {
            int index = MenuHelper.ShowMenuUpDown(
                options,
                $"=== Admin: Manage theme ===\n\n{theme}"
            );

            switch (index)
            {
                case 0:
                    EditThemeName(theme);
                    break;

                case 1:
                    EditThemeDescription(theme);
                    break;

                case 2:
                    editing = false; // Back
                    break;
            }
        }
    }

    private static void EditThemeName(ThemeModel theme)
    {
        Console.Clear();
        Console.WriteLine($"Current name: {theme.Name}");
        Console.WriteLine("Enter new name:");

        string? newName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(newName))
        {
            Console.WriteLine("Name cannot be empty.");
            Thread.Sleep(1500);
            return;
        }

        theme.Name = newName;
        logic.UpdateTheme(theme);
    }

    private static void EditThemeDescription(ThemeModel theme)
    {
        Console.Clear();
        Console.WriteLine($"Current description: {theme.Course}");
        Console.WriteLine("Enter new description:");

        string? newCourse = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(newCourse))
        {
            Console.WriteLine("Description cannot be empty.");
            Thread.Sleep(1500);
            return;
        }

        theme.Course = newCourse;
        logic.UpdateTheme(theme);
    }

    private static void DishesInTheme(ThemeModel theme)
    {
        string[] options =
        {
            "Add dishes",
            "Delete dish",
            "Back"
        };
        bool managing = true;

        while (managing)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage dishes in theme ===");
            switch (index)
            {
                case 0:
                    AddDishesToTheme(theme);
                    break;

                case 1:
                    DeleteDishesFromTheme(theme);
                    break;

                case 2:
                    managing = false;
                    break;
                }
        }
    }

    private static DishModel? ChooseDishes(List<DishModel> dishes)
    {
        bool selectingDish = true;
        int selected = 0;
        List<string> hidden = [];
        DisplayDishes(dishes, selected, hidden);
        List<DishModel> visible = logic.hidefilter(dishes, hidden);
        while (selectingDish)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Enter) 
            {
                selectingDish = false;
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
        Dictionary<int, List<ThemeModel>> dishThemes = dishes.ToDictionary(
            d => d.ID,
            d => logic.themesLinkedToDish(d)
        );
        
        for (int i = 0; i < dishes.Count; i++)
        {
            DishModel dish = dishes[i];

            string name = dish.Name.Length > 20 ? dish.Name[..17] + "..." : dish.Name;
            string type = dish.Type.Length > 12 ? dish.Type[..9] + "..." : dish.Type;

            List<ThemeModel> themes = dishThemes[dish.ID];
            
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
            return;
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
            return;
        }
        else
        {
            logic.DeleteDishonTheme(chosendish, theme);
            Console.Clear();
            Console.WriteLine("Deleted dish on theme");
            Thread.Sleep(1500);
            return;
        }

    }


    private static void Delete(ThemeModel theme)
    {
        if (theme == null) return;
        string[] options =
            {
                "Yes",
                "No"
            };
            bool deleting = true;
            while (deleting)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: delete theme ===\n\n{theme.ToString()}\n\nDelete Theme?");
            
            switch (index)
            {
                case 0:
                    logic.DeleteThemeCompletely(theme);
                    Console.WriteLine($"Theme: {theme.Name} deleted");
                    Thread.Sleep(1500);
                    deleting = false;
                    break;

                case 1:
                    deleting = false;
                    break;
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
        bool managingThemeCalendar = true;
        while (managingThemeCalendar)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage theme calendar ===");    
            switch (index)
                {
                    case 0:
                        ViewMenu.Start();
                        break;
                    case 1:
                        LinkMonthsToTheme();
                        break;
                    case 2:
                        managingThemeCalendar = false;
                        break;
                    }
            }   
    }

    private static void LinkMonthsToTheme()
    {
        ThemeModel? selectedTheme = ManageAllThemes();
        if (selectedTheme is null)
        {
            return; // user pressed esc in ManageAllThemes
        }

        // Get all available months
        List<string> monthOptions = logic.GetAvailableMonths();
        monthOptions.Add("Done");

        List<string> chosenMonths = new List<string>();
        bool choosingMonths = true;

        while (choosingMonths)
        {
            int index = MenuHelper.ShowMenuUpDown(
                monthOptions.ToArray(),
                "=== Admin: Choose months ==="
            );

            string selected = monthOptions[index];

            if (selected == "Done")
            {
                choosingMonths = false;
                continue;
            }

            if (chosenMonths.Contains(selected))
            {
                Console.Clear();
                Console.WriteLine($"This month is already selected: {selected}");
                Thread.Sleep(1500);
                continue;
            }

            Console.Clear();
            Console.WriteLine($"Added: {selected}");

            chosenMonths.Add(
                DateTime.ParseExact(selected, "MM-yyyy", null)
                        .ToString("yyyy-MM")
            );

            monthOptions.Remove(selected);

            if (monthOptions.Count == 1)
            {
                choosingMonths = false;
            }

            Thread.Sleep(1500);
            
        }

        Console.Clear();
        Console.WriteLine("Saved final month selection");
        Console.WriteLine("Final month selection:");

        foreach (string month in chosenMonths)
        {
            Console.WriteLine(" - " + month);
        }

        Thread.Sleep(3000);

        logic.LinkMonthsToTheme(chosenMonths, selectedTheme);

        Thread.Sleep(1500);
    }
}
