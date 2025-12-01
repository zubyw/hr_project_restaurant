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
    public static DateTime SelectDateArrowsLeftRight(bool includeDays = true)
    {
        DateTime minDate = DateTime.Now;

        DateTime current = DateTime.Now;

        int startTop = Console.CursorTop;

        Console.WriteLine();  

        while (true)
        {
            Console.SetCursorPosition(0, startTop);
            string leftArrow = current > minDate ? "< " : "  ";
            string rightArrow = " >";

            string line = includeDays
                ? $" Selected: {leftArrow} {current:dd-MM-yyyy} {rightArrow}"
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
