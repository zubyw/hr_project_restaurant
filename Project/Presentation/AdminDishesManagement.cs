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
            "Create new dish",
            "Manage all dishes",
            "Back"
        };

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Dishes Management ===");
        try
        {
            switch (index)
            {
                case 0:
                    Add();
                    break;

                case 1:
                    ManageAllDishes();
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

    private static void Add()
    {
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        
        Console.Write("Name: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Name cannot be empty.");
        Add();
        }
        if(_logic.DoesDishExist(name))
        {
            Console.WriteLine("There akready is a dish with this name");
            Thread.Sleep(1500);
            Start();
        }
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        Console.Write("Price (example: 12.50): ");
        string priceInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Price cannot be empty.");
        Add();
        }
        decimal price = decimal.Parse(priceInput.Replace(',', '.'),System.Globalization.CultureInfo.InvariantCulture);
        
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        Console.Write("Description: ");
        string description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Name cannot be empty.");
        Add();
        }


        Console.Clear();
        Console.WriteLine("=== Create new dish ===\nType (Starter/Main/Dessert): ");
        string[] types = { "Starter", "Main", "Dessert" };
        int typeIndex = MenuHelper.ShowMenuUpDown(types, "=== Create new dish ===\nType (Starter/Main/Dessert): ");
        string type = types[typeIndex];
        DishModel dish = new DishModel(){Name = name,Price = price, Description = description, Type = type}; 
        _logic.WriteIntoDB(dish);

        Console.WriteLine("Dish created.");
        Thread.Sleep(1500);
    }

    private static void ManageAllDishes()
    {
        List<DishModel> dishes = _logic.GetAllDishes();

        if (dishes.Count == 0)
        {
            Console.WriteLine("No dishes found.");
            Console.ReadKey();
            return;
        }

        int index = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== All Dishes ===\n");

            Console.WriteLine("┌────────────────────────┬───────────────┬───────────┐");
            Console.WriteLine("│ Name                   │ Type          │ Price     │");
            Console.WriteLine("├────────────────────────┼───────────────┼───────────┤");

            for (int i = 0; i < dishes.Count; i++)
            {
                var dish = dishes[i];

                string name = dish.Name.Length > 20 ? dish.Name[..17] + "..." : dish.Name;
                string type = dish.Type.Length > 12 ? dish.Type[..9] + "..." : dish.Type;

                bool selected = (i == index);

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine("│ {0,-22} │ {1,-13} │ {2,9:F2} │",
                    name, type, dish.Price);

                if (selected)
                    Console.ResetColor();
            }

            Console.WriteLine("└────────────────────────┴───────────────┴───────────┘");

            Console.WriteLine("\n ↑↓ Select dish | ESC = back");

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow)
            {
                index = (index + 1) % dishes.Count;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                index = (index - 1 + dishes.Count) % dishes.Count;
            }
            else if (key == ConsoleKey.Enter)
            {
                DishModel selectedDish = dishes[index];
                ManageDish(selectedDish);
                return;
            }
            else if (key == ConsoleKey.Escape)
            {
                return;
            }
        }
    }

    private static void ManageDish(DishModel dish)
    {
        string[] options =
        {
            "Edit dish",
            "Delete dish",
            "Back"
        };

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Manage dish ===");
        try
        {
            switch (index)
            {
                case 0:
                    Add();
                    break;

                case 1:
                    ManageAllDishes();
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
        // Console.WriteLine();
        // Console.WriteLine("┌────────┬───────────────────────┬────────────┬──────────┐");
        // Console.WriteLine("│  Name  │       Description     │    Type    │  Price   │");
        // Console.WriteLine("├────────┼───────────────────────┼────────────┼──────────┤");
