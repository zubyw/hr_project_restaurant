using Project.DataModels;
using Project.DataAccess;
using Project.Logic;

namespace Project.Presentation
{
    public static class ViewMenu
    {
        private static DishLogic _dishLogic = new DishLogic();
        private static ThemesLogic _themeLogic = new ThemesLogic();

        public static void Start()
        {
            Dictionary<string, int> themes = _themeLogic.GetAllActiveDatesAndThemes(); // MonthYear => ThemeId
            if (themes == null || themes.Count == 0)
            {
                ColorConsole.WriteError("No active menu theme available at this time.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            // Convert keys to a list so we can index by currentIndex
            var monthKeys = themes.Keys.ToList();

            int currentIndex = 0;
            int startTop = Console.CursorTop;

            while (true)
            {
                Console.SetCursorPosition(0, startTop);
                Console.Clear(); // clear once per loop

                string currentMonth = monthKeys[currentIndex];

                // Decide which arrows to show
                string leftArrow = currentIndex > 0 ? "< " : "  ";
                string rightArrow = currentIndex < monthKeys.Count - 1 ? " >" : "  ";

                // Display month selector
                Console.WriteLine($"Date : {leftArrow}{currentMonth}{rightArrow}");
                Console.WriteLine();

                int themeId = themes[currentMonth];
                var theme = _themeLogic.GetById(themeId);

                if (theme != null)
                {
                    DisplayThemeMenu(theme); // just print dishes, don't clear
                }
                else
                {
                    Console.WriteLine($"No theme available for {currentMonth}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        if (currentIndex > 0) currentIndex--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (currentIndex < monthKeys.Count - 1) currentIndex++;
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private static void DisplayThemeMenu(ThemeModel theme)
        {
            // Get all dishes for this theme
            List<DishModel> dishes = _dishLogic.GetDishesByTheme(theme.ID);
            

            // Separate by type
            var starters = dishes.Where(d => d.Type == "Starter").ToList();
            var mains = dishes.Where(d => d.Type == "Main").ToList();
            var desserts = dishes.Where(d => d.Type == "Dessert").ToList();

            // Header
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {theme.Name.ToUpper()}".PadRight(77) + "║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Display Starters
            if (starters.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("═══ STARTERS ═══");
                Console.ResetColor();
                Console.WriteLine();
                foreach (var dish in starters)
                {
                    DisplayDish(dish);
                }
                Console.WriteLine();
            }

            // Display Main Courses
            if (mains.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("═══ MAIN COURSES ═══");
                Console.ResetColor();
                Console.WriteLine();
                foreach (var dish in mains)
                {
                    DisplayDish(dish);
                }
                Console.WriteLine();
            }

            // Display Desserts
            if (desserts.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("═══ DESSERTS ═══");
                Console.ResetColor();
                Console.WriteLine();
                foreach (var dish in desserts)
                {
                    DisplayDish(dish);
                }
                Console.WriteLine();
            }

            if (dishes.Count == 0)
            {
                ColorConsole.WriteError("No dishes available for this theme.");
                Console.WriteLine();
            }

            Console.WriteLine(new string('─', 80));
            Console.WriteLine("\nPress ESC to return...");
        }

        private static void DisplayDish(DishModel dish)
        {
            Console.WriteLine($"  {dish.Name}".PadRight(50) + $"€{dish.Price:F2}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"    {dish.Description}");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
