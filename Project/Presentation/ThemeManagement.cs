using System;
using System.Collections.Generic;

public static class ThemeManagement
{
    private static readonly ThemesLogic logic = new ThemesLogic();

    public static void Start()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Theme Management ===");
            Console.WriteLine("1) Show all themes");
            Console.WriteLine("2) Create new theme");
            Console.WriteLine("3) Update theme");
            Console.WriteLine("4) Activate theme");
            Console.WriteLine("5) Deactivate theme");
            Console.WriteLine("6) Delete theme completely");
            Console.WriteLine("0) Back to admin menu");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            try
            {
                if (choice == "1") ShowAll();
                else if (choice == "2") Create();
                else if (choice == "3") Update();
                else if (choice == "4") Activate();
                else if (choice == "5") Deactivate();
                else if (choice == "6") Delete();
                else if (choice == "0") ReservationManagement.Start();
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
            Console.WriteLine("┌──────┬─────────────────────┬──────────────┬──────────┐");
            Console.WriteLine("│  ID  │        Name         │    Course    │  Active  │");
            Console.WriteLine("├──────┼─────────────────────┼──────────────┼──────────┤");

            foreach (ThemeModel t in list)
            {
                string active = t.IsActive == 1 ? "Yes" : "No";
                Console.WriteLine($"│ {t.ID,4} │ {t.Name,-19} │ {t.Course,-12} │ {active,-8} │");
            }

            Console.WriteLine("└──────┴─────────────────────┴──────────────┴──────────┘");
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
        Console.Write("Course (Starter/Main/Dessert): ");
        string course = Console.ReadLine();
        Console.Write("Month date (yyyy-MM-dd): ");
        DateTime time = DateTime.Parse(Console.ReadLine());

        logic.CreateTheme(name, course, time);
        Console.WriteLine("✓ Theme created successfully.");
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
        Console.Write("New course (Starter/Main/Dessert): ");
        string course = Console.ReadLine();
        Console.Write("Active (1 or 0): ");
        int active = int.Parse(Console.ReadLine());

        logic.UpdateTheme(id, name, course, active);
        Console.WriteLine("✓ Theme updated successfully.");
        Console.ReadKey();
    }

    private static void Activate()
    {
        Console.Clear();
        Console.WriteLine("=== Activate Theme ===");
        Console.Write("Theme ID: ");
        int id = int.Parse(Console.ReadLine());
        logic.Activate(id);
        Console.WriteLine("✓ Theme activated.");
        Console.ReadKey();
    }

    private static void Deactivate()
    {
        Console.Clear();
        Console.WriteLine("=== Deactivate Theme ===");
        Console.Write("Theme ID: ");
        int id = int.Parse(Console.ReadLine());
        logic.Deactivate(id);
        Console.WriteLine("✓ Theme deactivated.");
        Console.ReadKey();
    }

    private static void Delete()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Theme ===");
        Console.Write("Theme ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        logic.DeleteThemeCompletely(id);
        Console.WriteLine("✓ Theme deleted.");
        Console.ReadKey();
    }
}
