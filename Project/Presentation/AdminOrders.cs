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

        string input = CalanderInput.Calander();

        List<(string DishName, int Count)> dishCounts = _dishLogic.GetDishCountsForDate(input);

        if (dishCounts.Count == 0)
        {
            Console.WriteLine("No orders found for this date.");
        }
        else
        {
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
