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
            Dictionary<string, int> themes = _themeLogic.GetAllActiveDatesAndThemes();
            if (themes == null || themes.Count == 0)
            {
                ColorConsole.WriteError("No active menu theme available at this time.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var monthKeys = themes.Keys.ToList();

            int currentIndex = 0;
            int startTop = Console.CursorTop;

            while (true)
            {
                Console.SetCursorPosition(0, startTop);
                Console.Clear();

                string currentMonth = monthKeys[currentIndex];

                string leftArrow = currentIndex > 0 ? "< " : "  ";
                string rightArrow = currentIndex < monthKeys.Count - 1 ? " >" : "  ";

                Console.WriteLine($"Date : {leftArrow}{currentMonth}{rightArrow}");
                Console.WriteLine();

                int themeId = themes[currentMonth];
                var theme = _themeLogic.GetById(themeId);

                if (theme != null)
                {
                    DisplayThemeMenu(theme);
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
            List<DishModel> dishes = _dishLogic.GetDishesByTheme(theme.ID);

            var starters = dishes.Where(d => d.Type == "Starter").ToList();
            var mains = dishes.Where(d => d.Type == "Main").ToList();
            var desserts = dishes.Where(d => d.Type == "Dessert").ToList();

            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {theme.Name.ToUpper()}".PadRight(77) + "║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

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
            Console.WriteLine("\nPress ESC to return to the main menu...");
        }

        private static void DisplayDish(DishModel dish)
        {
            Console.WriteLine($"  {dish.Name}".PadRight(50) + $"€{dish.Price:F2}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"    {dish.Description}");
            
            if (dish.AllergenNames.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Contains: {string.Join(", ", dish.AllergenNames)}");
            }
            
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
