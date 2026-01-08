using Project.Presentation;

static class Menu
{
    // Store current logged in user for role-based access
    public static UserModel? CurrentUser { get; set; }

    //This shows the menu. You can call back to this method to show the menu again
    //after another presentation method is completed.
    //You could edit this to show different menus depending on the user's role
    static public void Start()
    {
        string[] options = new string[] { "Login", "Register", "Exit" };
        int selectedIndex = 0;

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine("\n=== Kevin's Fine Dining - Authentication ===");

            // Display options
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"  {options[i]}");
                Console.ResetColor();
            }

            key = Console.ReadKey(true).Key;

            // Handle arrow keys
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    break;
                case ConsoleKey.Enter:
                    switch (selectedIndex)
                    {
                        case 0:
                            UserLogin.Start();
                            break;
                        case 1:
                            UserRegistration.Start();
                            break;
                        case 2:
                            Environment.Exit(0);
                            break;
                    }
                    break;
            }
        } while (key != ConsoleKey.Enter);
    }

    // This menu shows after successful login
    static public void ShowMainMenu()
    {
        Console.Clear();

        // Check if user is admin or regular user
        bool isAdmin = IsCurrentUserAdmin();

        if (isAdmin)
        {
            ShowAdminMenu();
        }
        else
        {
            ShowCustomerMenu();
        }
    }

    // Admin menu with full access
    static public void ShowAdminMenu()
    {
        string[] options = new string[] { "Manage Reservations", "Dish Orders", "Manage Themes", "Manage Dishes", "Manage Drinks", "Logout" };
        bool inMenu = true;
        while (inMenu)
        {
            int index = MenuHelper.ShowMenuUpDown(options, "=== Kevin's Fine Dining - Admin Panel ===");
            switch (index)
            {
                case 0:
                    ReservationManagement.Start();
                    break;
                case 1:
                    DishOrderOverview.Start();
                    break;
                case 2:
                    ThemeManagement.Start();
                    break;
                case 3:
                    AdminDishesManagement.Start();
                    break;
                case 4:
                    AdminDrinksManagement.Start();
                    break;
                case 5:
                    inMenu = false;
                    Start();
                    break;
            }
        }
    }

    // Customer menu with limited access
    static public void ShowCustomerMenu()
    {
        string[] options = new string[] { "Make a Reservation", "View My Reservations", "View Menu", "Logout" };
        bool inMenu = true;

        while (inMenu)
        {
            int selectedIndex = MenuHelper.ShowMenuUpDown(
                options, 
                $"=== Welcome {CurrentUser?.FirstName} {CurrentUser?.LastName} ==="
            );

            switch (selectedIndex)
            {
                case 0:
                    UserReservation.Start();
                    break;
                case 1:
                    if (CurrentUser != null)
                    {
                        RudReservation rud = new RudReservation();
                        rud.Start();
                    }
                    break;
                case 2:
                    ViewMenu.Start();
                    break;
                case 3:
                    Console.WriteLine("Logging out...");
                    CurrentUser = null;
                    inMenu = false;
                    Start();
                    break;
            }
        }
    }



    // Helper method to check if current user is admin
    private static bool IsCurrentUserAdmin()
    {
        if (CurrentUser == null) return false;

        // Check if user is admin by email or by getting user role from Users table
        if (CurrentUser.EmailAddress == "admin@gmail.com") return true;

        try
        {
            var usersAccess = new UsersAccess();
            var user = usersAccess.GetByEmail(CurrentUser.EmailAddress);
            return user?.Roles?.Contains("admin") == true;
        }
        catch
        {
            return false;
        }
    }
}
