public static class MenuHelper
{
    public static int ShowMenuUpDown(string[] options, string title = "", int selectedIndex = 0)
    {
        ConsoleKey key;
        bool choosing = true;
        while (choosing)
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
                    choosing = false;
                    break;
            }
        }
        return selectedIndex;
    }
}
