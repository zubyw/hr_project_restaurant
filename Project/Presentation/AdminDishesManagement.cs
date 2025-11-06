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
}
