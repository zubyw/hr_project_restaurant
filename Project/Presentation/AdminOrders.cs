using System;
using System.Collections.Generic;
using Project.Logic;

static class DishOrderOverview
{
    private static DishLogic _dishLogic = new DishLogic();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("=== Dish Orders Overview ===\n");

        Console.Write("Enter date (dd-MM-yyyy): ");
        string? input = Console.ReadLine()?.Trim();

        if (!DateTime.TryParseExact(input, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date format. Press any key to return...");
            Console.ReadKey();
            return; // terug naar het admin-menu
        }

        List<(string DishName, int Count)> dishCounts = _dishLogic.GetDishCountsForDate(input);

        if (dishCounts.Count == 0)
        {
            Console.WriteLine("No orders found for this date.");
        }
        else
        {
            Console.WriteLine($"\nOrders for {input}:\n");
            foreach (var item in dishCounts)
            {
                Console.WriteLine($"{item.DishName} - {item.Count}");
            }
        }

        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
        return; // terug naar het admin-menu
    }
}
