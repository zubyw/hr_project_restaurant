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
            Console.WriteLine("Use Arrow up  & Arrow Down to change (0.1)");
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
}
