using System;
using System.Collections.Generic;

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
                    ManageAllThemes();
                    break;

                case 2:
                    // ManageThemeCalandar();
                    // break;
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
        string name = Console.ReadLine();
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
        string description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Description cannot be empty.");
        Add();
        }

        ThemeModel theme = new ThemeModel(){Name = name, Course = description};
        logic.WriteTheme(theme);
        Start();
    }


    private static void ManageAllThemes()
    {
        Console.Clear();
        List<ThemeModel> themes = logic.GetAll();

        if (themes.Count == 0)
        {
            Console.WriteLine("No themes found.");
            return;
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
                ManageTheme(selectedTheme);
            }
            else if (key == ConsoleKey.Escape)
            {
                Start();
            }
        }
    }


    private static void ManageTheme(ThemeModel theme)
    {
        string[] options =
        {
            "Edit theme",
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
                    return;

                case 1:
                    // Delete(theme);
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
                    string newname = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newname))
                    {
                    Console.WriteLine("Name cannot be empty.");
                    Thread.Sleep(1500);
                    continue;
                    }
                    theme.Name = newname;
                    logic.UpdateTheme(theme);
                    break;

                case 1:
                    Console.Clear();
                    Console.WriteLine($"Description: {theme.Course}");
                    Console.WriteLine();
                    string newcourse = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newcourse))
                    {
                    Console.WriteLine("Name cannot be empty.");
                    Thread.Sleep(1500);
                    continue;
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


    private static void Delete(ThemeModel theme)
    {
        string[] options =
            {
                "Yes",
                "No"
            };

            while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: delete theme ===\n\n{theme.ToString()}\n\nDelete dish?");
            try
            {
                switch (index)
                {
                    case 0:
                        logic.DeleteThemeCompletely(theme);
                        Console.WriteLine($"Dish: {theme.Name} deleted");
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
}
