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
                else if (choice == "0") break;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }
        }
    }
}
