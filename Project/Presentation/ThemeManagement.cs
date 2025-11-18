using System;
using System.Collections.Generic;

public static class ThemeManagement
{
    private static readonly ThemesLogic logic = new ThemesLogic();

    public static void Start()
    {
        string[] options =
        {
            "Show all themes",
            "Create new theme",
            "Update theme",
            "Activate theme",
            "Deactivate theme",
            "Delete theme completely",
            "Back to admin menu"
        };

        int index = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Theme Management ===");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == index)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(options[i]);
                Console.ResetColor();
            }

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                index--;
                if (index < 0) index = options.Length - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                index++;
                if (index >= options.Length) index = 0;
            }
            else if (key == ConsoleKey.Enter)
            {
                try
                {
                    switch (index)
                    {
                        case 0:
                            ShowAll();
                            break;
                        case 1:
                            Create();
                            break;
                        case 2:
                            Update();
                            break;
                        case 3:
                            Activate();
                            break;
                        case 4:
                            Deactivate();
                            break;
                        case 5:
                            Delete();
                            break;
                        case 6:
                            ReservationManagement.Start();
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


    private static void Create()
    {
        Console.Clear();
        Console.WriteLine("=== Create Theme ===");
        Console.Write("Theme name: ");
        string name = Console.ReadLine();
        Console.Write("Course: ");
        string course = Console.ReadLine();

        DateTime time;
        while (true)
        {
            Console.Write("Month date (yyyy-MM-dd): ");
            string input = Console.ReadLine();
            if (DateTime.TryParse(input, out time))
            {
                break;
            }
            Console.WriteLine("Invalid date format, try again.");
        }

        logic.CreateTheme(name, course, time);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Theme created successfully.");
        Console.ResetColor();
        Console.ReadKey();
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

        DateTime monthDate;
        while (true)
        {
            Console.Write("New month date (yyyy-MM-dd): ");
            string input = Console.ReadLine();
            if (DateTime.TryParse(input, out monthDate))
            {
                break;
            }
            Console.WriteLine("Invalid date format, try again.");
        }

        logic.UpdateTheme(id, name, course, active);

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
