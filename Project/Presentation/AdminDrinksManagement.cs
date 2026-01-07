using Project.Logic;
using Project.DataModels;

public static class AdminDrinksManagement
{
    public static void Start()
    {
        string[] options =
        {
            "Create new drink",
            "Manage drinks",
            "Back"
        };

        while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(
                options,
                "=== Admin: Drinks Management ==="
            );

            switch (index)
            {
                case 0:
                    CreateDrink();
                    break;

                case 1:
                    ManageDrinks(); 
                    break;

                case 2:
                    return;
            }
        }
    }

    private static void CreateDrink()
    {
        Console.Clear();
        Console.WriteLine("=== Create new drink ===");

        Console.Write("Name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Price (€): ");
        string priceInput = Console.ReadLine() ?? "";

        double alcoholPercentage = SelectAlcoholPercentage();

        try
        {
            decimal price = decimal.Parse(
                priceInput.Replace(',', '.'),
                System.Globalization.CultureInfo.InvariantCulture
            );

            DrinkLogic logic = new DrinkLogic();
            logic.CreateDrink(name, price, alcoholPercentage);

            Console.WriteLine();
            Console.WriteLine("Drink created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
        }

        Thread.Sleep(1500);
    }


    private static double SelectAlcoholPercentage()
    {
        double percentage = 0.0;
        bool selecting = true;

        while (selecting)
        {
            Console.Clear();
            Console.WriteLine("=== Alcohol percentage ===");
            Console.WriteLine();
            Console.WriteLine("Use Arrow Up & Arrow Down to set % with 0.1");
            Console.WriteLine("ENTER to confirm");
            Console.WriteLine();
            Console.WriteLine($"Alcohol: {percentage:F1} %");

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (percentage < 100.0)
                        percentage = Math.Round(percentage + 0.1, 1);
                    break;

                case ConsoleKey.DownArrow:
                    if (percentage > 0.0)
                        percentage = Math.Round(percentage - 0.1, 1);
                    break;

                case ConsoleKey.Enter:
                    selecting = false;
                    break;
            }
        }

        return percentage;
    }

    private static void ManageDrinks()
    {
        DrinkLogic logic = new DrinkLogic();
        List<Drink> drinks = logic.GetAllDrinks();

        if (drinks.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No drinks found.");
            Console.ReadKey();
            return;
        }

        int index = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Manage Drinks ===");
            Console.WriteLine();
            Console.WriteLine("Use Arrow up & Arrow down to set % with 0.1 | ENTER = details | ESC = back");
            Console.WriteLine();

            for (int i = 0; i < drinks.Count; i++)
            {
                bool selected = i == index;

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Drink d = drinks[i];
                Console.WriteLine(
                    $"{d.Name.PadRight(25)}  {d.AlcoholPercentage,5:F1}%   €{d.Price,6:F2}"
                );

                if (selected)
                    Console.ResetColor();
            }

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    index = (index - 1 + drinks.Count) % drinks.Count;
                    break;

                case ConsoleKey.DownArrow:
                    index = (index + 1) % drinks.Count;
                    break;

                case ConsoleKey.Enter:
                    ShowDrinkDetails(drinks[index]);
                    break;

                case ConsoleKey.Escape:
                    return;
            }
        }
    }
    
    private static void ShowDrinkDetails(Drink drink)
    {
        string[] options =
        {
            "Edit name",
            "Edit price",
            "Edit alcohol percentage",
            "Delete drink",
            "Back"
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Drink Details ===");
            Console.WriteLine();
            Console.WriteLine($"Name:     {drink.Name}");
            Console.WriteLine($"Alcohol:  {drink.AlcoholPercentage:F1}%");
            Console.WriteLine($"Price:    €{drink.Price:F2}");
            Console.WriteLine();

            int choice = MenuHelper.ShowMenuUpDown(options, "Select an option");

            switch (choice)
            {
                case 0:
                    EditDrinkName(drink);
                    break;

                case 1:
                    EditDrinkPrice(drink);
                    break;

                case 2:
                    EditDrinkAlcohol(drink);
                    break;

                case 3:
                    DeleteDrink(drink);
                    return;

                case 4:
                    return;
            }
        }
    }

    private static void EditDrinkName(Drink drink)
    {
        Console.Clear();
        Console.Write("New name: ");
        string newName = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(newName))
            return;

        drink.Name = newName;

        new DrinkLogic().UpdateDrink(drink);
    }

    private static void EditDrinkPrice(Drink drink)
    {
        Console.Clear();
        Console.Write("New price (€): ");
        string input = Console.ReadLine() ?? "";

        if (!decimal.TryParse(
            input.Replace(',', '.'),
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal price))
            return;

        drink.Price = price;

        new DrinkLogic().UpdateDrink(drink);
    }

    private static void EditDrinkAlcohol(Drink drink)
    {
        double percentage = drink.AlcoholPercentage;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Edit alcohol percentage");
            Console.WriteLine("Use Arrow Up & Arrow Down to set % with 0.1");
            Console.WriteLine("ENTER to save");
            Console.WriteLine();
            Console.WriteLine($"Alcohol: {percentage:F1}%");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && percentage < 100)
                percentage = Math.Round(percentage + 0.1, 1);
            else if (key == ConsoleKey.DownArrow && percentage > 0)
                percentage = Math.Round(percentage - 0.1, 1);
            else if (key == ConsoleKey.Enter)
                break;
        }

        drink.AlcoholPercentage = percentage;
        new DrinkLogic().UpdateDrink(drink);
    }

    private static void DeleteDrink(Drink drink)
    {
        Console.Clear();
        Console.WriteLine($"Delete '{drink.Name}'?");
        Console.Write("Are you sure? (y/n): ");

        if (Console.ReadLine()?.ToLower() != "y")
            return;

        bool success = new DrinkLogic().DeleteDrink(drink.ID);

        Console.WriteLine(success
            ? "Drink deleted."
            : "Drink is linked to a dish and cannot be deleted.");

        Thread.Sleep(1500);
    }
}
