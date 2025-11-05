using Project.DataModels;
using Project.DataAccess;

namespace Project.Presentation
{
    public static class ViewMenu
    {
        private static DishAccess _dishAccess = new DishAccess();
        private static ThemeAccess _themeAccess = new ThemeAccess();

        public static void Start()
        {
            Console.Clear();
            DisplayMenuHeader();

            // Get active theme
            int? activeThemeId = _themeAccess.GetActiveThemeID();

            if (activeThemeId == null)
            {
                ColorConsole.WriteError("No active menu theme available at this time.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            // Get the theme details
            var theme = _themeAccess.GetById(activeThemeId.Value);

            if (theme == null)
            {
                ColorConsole.WriteError("Unable to load menu theme.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            // Display the active theme menu
            DisplayThemeMenu(theme);
        }

        private static void DisplayMenuHeader()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    🍽️  KEVIN'S FINE DINING - MENU  🍽️                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        private static void DisplayThemeMenu(ThemeModel theme)
        {
            Console.Clear();
            
            // Get all dishes for this theme
            var dishIds = _dishAccess.GetallDishIdByThemeId(theme.ID);
            var dishes = dishIds.Count > 0 ? _dishAccess.GetDishesByIds(dishIds) : new List<DishModel>();

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
            Console.WriteLine("\nPress any key to return to theme selection...");
            Console.ReadKey();
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
