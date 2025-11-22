public static class MenuHelper
{
    public static int ShowMenuUpDown(string[] options, string title = "", int selectedIndex = 0)
    {
        ConsoleKey key;

        while (true)
        {
            Console.Clear();
            Console.WriteLine(title);
            Console.WriteLine();
            // Draw menu
            for (int i = 0; i < options.Length; i++)
            {
                bool isSelected = (i == selectedIndex);

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"  {options[i]}");

                if (isSelected)
                    Console.ResetColor();
            }

            // Wait for key
            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    break;

                case ConsoleKey.Enter:
                    return selectedIndex;
            }
        }
    }
    public static DateTime SelectDateArrowsLeftRight(DateTime initialDate, DateTime minDate, bool includeDays = true)
    {
        DateTime current = initialDate;

        int startTop = Console.CursorTop;

        Console.WriteLine();  

        while (true)
        {
            Console.SetCursorPosition(0, startTop);

            string line = includeDays
                ? $" Selected: {current:dd-MM-yyyy} "
                : $" Selected: {current:MM-yyyy} ";

            Console.WriteLine(line.PadRight(Console.WindowWidth));

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    current = includeDays ? current.AddDays(-1) : current.AddMonths(-1);
                    break;

                case ConsoleKey.RightArrow:
                    current = includeDays ? current.AddDays(1) : current.AddMonths(1);
                    break;

                case ConsoleKey.Enter:
                    Console.SetCursorPosition(0, startTop + 1);
                    return current;
            }

            if (current < minDate)
                current = minDate;
        }
    }
}
