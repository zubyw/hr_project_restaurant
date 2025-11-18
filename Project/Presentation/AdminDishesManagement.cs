using System;
using System.Collections.Generic;
using Project.Logic;
using Project.DataModels;

public static class AdminDishesManagement
{
    private static readonly DishLogic _logic = new DishLogic();

    public static void Start()
    {
        string[] options =
        {
            "Show dishes by theme",
            "Add dish to theme",
            "Update dish in theme",
            "Delete dish from theme",
            "Back"
        };

        int index = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Admin: Dishes Management ===");

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
                            ShowByTheme();
                            break;
                        case 1:
                            Add();
                            break;
                        case 2:
                            Update();
                            break;
                        case 3:
                            Delete();
                            break;
                        case 4:
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
        Console.WriteLine("┌────────┬──────────┬──────────────────────┬────────────┬──────────┐");
        Console.WriteLine("│   ID   │  Theme   │         Name         │    Type    │  Price   │");
        Console.WriteLine("├────────┼──────────┼──────────────────────┼────────────┼──────────┤");

        foreach (DishModel d in list)
        {
            string name = d.Name.Length > 22 ? d.Name.Substring(0, 22) : d.Name;
            string type = d.Type.Length > 10 ? d.Type.Substring(0, 10) : d.Type;
            string price = d.Price.ToString("0.00").Replace('.', ',');

            Console.WriteLine(
                $"│ {d.ID,6} │ {themeId,8} │ {name,-21} │ {type,-9} │ {price,7} │");
        }

        Console.WriteLine("└────────┴──────────┴──────────────────────┴────────────┴──────────┘");
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
