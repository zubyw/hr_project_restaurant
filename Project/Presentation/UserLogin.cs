static class UserLogin
{
    static public UserModel activeUser { get; set; }
    static private UsersLogic usersLogic = new UsersLogic();


    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n=== Login ===");
        Console.Write("Email address: ");
        string? email = Console.ReadLine();
        Console.Write("Password: ");
        string? password = Console.ReadLine();
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Email and password are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Menu.Start();
            return;
        }
        
        UserModel? user = usersLogic.CheckLogin(email, password);
        if (user != null)
        {
            // Set current user for role-based access
            Menu.CurrentUser = user;
            
            Console.WriteLine($"Welcome back {user.FirstName} {user.LastName}!");
            Console.WriteLine("Login successful. Redirecting to main menu...");
            
            // Small delay to show the welcome message
            Thread.Sleep(1500);
            activeUser = usersLogic.GetUserByEmail(email);
            Menu.ShowMainMenu();
        }
        else
        {
            Console.WriteLine("Invalid email or password!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Menu.Start();
        }
    }
}