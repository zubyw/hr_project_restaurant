using System;
using System.Collections.Generic;
using Project.Logic;
using Project.DataModels;

public static class AdminDishesManagement
{
    private static readonly DishLogic _logic = new DishLogic();

    public static void Start()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Admin: Dishes Management ===");
            Console.WriteLine("1) Show dishes by theme");
            Console.WriteLine("2) Add dish to theme");
            Console.WriteLine("3) Update dish in theme");
            Console.WriteLine("4) Delete dish from theme");
            Console.WriteLine("0) Back");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            try
            {
                if (choice == "1")
                {
                    ShowByTheme();
                }
                else if (choice == "2")
                {
                    Add();
                }
                else if (choice == "3")
                {
                    Update();
                }
                else if (choice == "4")
                {
                    Delete();
                }
                else if (choice == "0")
                {
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

    private static void ShowByTheme()
    {
        Console.Clear();
        int themeId = AskInt("Theme ID: ");

        List<DishModel> list = _logic.AdminGetDishesByTheme(themeId);

        if (list.Count == 0)
        {
            Console.WriteLine("No dishes found.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("┌──────┬────────┬──────────────────────┬──────────┬────────────────────────────┐");
        Console.WriteLine("│  ID  │ Theme  │        Name          │  Type    │           Price/Desc       │");
        Console.WriteLine("├──────┼────────┼──────────────────────┼──────────┼────────────────────────────┤");
        foreach (DishModel d in list)
        {
            string price = d.Price.ToString("0.00");
            string nm = d.Name.Length > 22 ? d.Name.Substring(0, 22) + "..." : d.Name;
            string desc = d.Description.Length > 24 ? d.Description.Substring(0, 24) + "..." : d.Description;
            Console.WriteLine(string.Format(
                "│ {0,4} │ {1,6} │ {2,-22} │ {3,-8} │ {4,-8} {5,-15} │",
                d.ID, themeId, nm, d.Type, price, desc));
        }
        Console.WriteLine("└──────┴────────┴──────────────────────┴──────────┴────────────────────────────┘");
        Console.WriteLine();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    private static void Add()
    {
        Console.Clear();
        Console.WriteLine("=== Add Dish to Theme ===");
        int themeId = AskInt("Theme ID: ");
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Price (e.g. 12.50): ");
        decimal price = AskDecimal();
        Console.Write("Description: ");
        string description = Console.ReadLine();
        Console.Write("Type (Starter/Main/Dessert): ");
        string type = Console.ReadLine();

        int newId = _logic.AdminAddDishToTheme(themeId, name, price, description, type);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Dish created. ID: " + newId);
        Console.ResetColor();
        Console.ReadKey();
    }

    private static void Update()
    {
        Console.Clear();
        Console.WriteLine("=== Update Dish in Theme ===");
        int dishId = AskInt("Dish ID: ");
        int themeId = AskInt("Theme ID: ");
        Console.Write("New name: ");
        string name = Console.ReadLine();
        Console.Write("New price: ");
        decimal price = AskDecimal();
        Console.Write("New description: ");
        string description = Console.ReadLine();
        Console.Write("New type (Starter/Main/Dessert): ");
        string type = Console.ReadLine();

        _logic.AdminUpdateDishInTheme(dishId, themeId, name, price, description, type);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Dish updated.");
        Console.ResetColor();
        Console.ReadKey();
    }

    private static void Delete()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Dish from Theme ===");
        int dishId = AskInt("Dish ID: ");
        int themeId = AskInt("Theme ID: ");

        _logic.AdminDeleteDishFromTheme(dishId, themeId);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Dish deleted.");
        Console.ResetColor();
        Console.ReadKey();
    }

    private static int AskInt(string label)
    {
        Console.Write(label);
        string s = Console.ReadLine();
        int n;
        if (!int.TryParse(s, out n))
        {
            throw new Exception("Invalid number.");
        }
        return n;
    }

    private static decimal AskDecimal()
    {
        string s = Console.ReadLine();
        decimal v;
        if (!decimal.TryParse(s, out v))
        {
            throw new Exception("Invalid decimal.");
        }
        return v;
    }
}
