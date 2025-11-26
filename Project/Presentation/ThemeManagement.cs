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

    private static void ShowAll()
    {
        Console.Clear();
        List<ThemeModel> list = logic.GetAll();

        if (list.Count == 0)
        {
            Console.WriteLine("No themes found.");
        }
        else
        {
            Console.WriteLine();

            Console.WriteLine("┌──────┬────────────────────────────┬───────────────────────────────────────────────┬──────────┐");
            Console.WriteLine("│  ID  │           Name             │                    Course                     │  Active  │");
            Console.WriteLine("├──────┼────────────────────────────┼───────────────────────────────────────────────┼──────────┤");

            foreach (ThemeModel t in list)
            {
                string active = t.IsActive == 1 ? "Yes" : "No";

                string nameText = t.Name ?? "";
                if (nameText.Length > 26)
                nameText = nameText.Substring(0, 26);

                string courseText = t.Course ?? "";
                if (courseText.Length > 45)
                    courseText = courseText.Substring(0, 45);

                Console.WriteLine(
                    $"│ {t.ID,4} │ {nameText,-26} │ {courseText,-45} │ {active,-8} │");
            }

            Console.WriteLine("└──────┴────────────────────────────┴───────────────────────────────────────────────┴──────────┘");
            Console.WriteLine();
        }

        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
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
                // ThemeModel selectedTheme = themes[index];
                // ManageTheme(selectedTheme);
            }
            else if (key == ConsoleKey.Escape)
            {
                Start();
            }
        }
    }

    private static void Update()
    {
        Console.Clear();
        Console.WriteLine("=== Update Theme ===");
        Console.Write("Theme ID: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("New name: ");
        string name = Console.ReadLine();
        Console.Write("New course: ");
        string course = Console.ReadLine();
        Console.Write("Active (1 or 0): ");
        int active = int.Parse(Console.ReadLine());

        int year;
        while (true)
        {
            Console.Write("New year (yyyy, >= 2025): ");
            if (int.TryParse(Console.ReadLine(), out year))
            {
                break;
            }
            Console.WriteLine("Invalid year, try again.");
        }

        int month;
        while (true)
        {
            Console.Write("New month (1-12): ");
            if (int.TryParse(Console.ReadLine(), out month) &&
                month >= 1 && month <= 12)
            {
            break;
            }
            Console.WriteLine("Invalid month, try again.");
        }

        DateTime monthDate = new DateTime(year, month, 1);

        logic.UpdateTheme(id, name, course, active, monthDate);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Theme updated successfully.");
        Console.ResetColor();
        Console.ReadKey();
    }

    private static void Activate()
    {
        Console.Clear();
        Console.WriteLine("=== Activate Theme ===");
        Console.Write("Theme ID: ");
        int id = int.Parse(Console.ReadLine());
        logic.Activate(id);
        Console.WriteLine("Theme activated.");
        Console.ReadKey();
    }

    private static void Deactivate()
    {
        Console.Clear();
        Console.WriteLine("=== Deactivate Theme ===");
        Console.Write("Theme ID: ");
        int id = int.Parse(Console.ReadLine());
        logic.Deactivate(id);
        Console.WriteLine("Theme deactivated.");
        Console.ReadKey();
    }

    private static void Delete()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Theme ===");
        Console.Write("Theme ID to delete: ");
        int id = int.Parse(Console.ReadLine());

        try
        {
            logic.DeleteThemeCompletely(id);
            Console.WriteLine("Theme deleted.");
        }

        catch (Exception ex)
        {
            Console.WriteLine("Delete failed: " + ex.Message);
        }

        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }
}
