static class Menu
{
    // Store current logged in user for role-based access
    public static UserModel? CurrentUser { get; set; }

    //This shows the menu. You can call back to this method to show the menu again
    //after another presentation method is completed.
    //You could edit this to show different menus depending on the user's role
    static public void Start()
    {
        Console.Clear();
        Console.WriteLine("\n=== Kevin's Fine Dining - Authentication ===");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("3. Exit");
        Console.Write("Please select an option: ");

        string? input = Console.ReadLine();
        if (input == "1")
        {
            UserLogin.Start();
        }
        else if (input == "2")
        {
            UserRegistration.Start();
        }
        else if (input == "3")
        {
            Console.WriteLine("Thank you for using Kevin's Fine Dining System!");
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Start();
        }
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
        Console.WriteLine("\n=== Kevin's Fine Dining - Admin Panel ===");
        Console.WriteLine("1. Manage Users");
        Console.WriteLine("2. Manage Reservations");
        Console.WriteLine("3. Manage Tables");
        Console.WriteLine("4. Manage Dishes");
        Console.WriteLine("5. View Reports");
        Console.WriteLine("6. Logout");
        Console.Write("Please select an option: ");

        string? input = Console.ReadLine();
        switch (input)
        {
            case "1":
                UserManagement.Start();
                break;
            case "2":
                Console.WriteLine("Reservation management coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "3":
                Console.WriteLine("Table management coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "4":
                Console.WriteLine("Dish management coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "5":
                Console.WriteLine("Reports feature coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "6":
                Console.WriteLine("Logging out...");
                CurrentUser = null;
                Start(); // Go back to login/register menu
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
        }
    }

    // Customer menu with limited access
    static public void ShowCustomerMenu()
    {
        Console.WriteLine($"\n=== Welcome {CurrentUser?.FirstName} {CurrentUser?.LastName} ===");
        Console.WriteLine("1. Make a Reservation");
        Console.WriteLine("2. View My Reservations");
        Console.WriteLine("3. Update Profile");
        Console.WriteLine("4. Logout");
        Console.Write("Please select an option: ");

        string? input = Console.ReadLine();
        switch (input)
        {
            case "1":
                Console.WriteLine("Make reservation feature coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "2":
                Console.WriteLine("View reservations feature coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "3":
                Console.WriteLine("Update profile feature coming soon...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
            case "4":
                Console.WriteLine("Logging out...");
                CurrentUser = null;
                Start(); // Go back to login/register menu
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowMainMenu();
                break;
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