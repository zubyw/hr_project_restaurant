static class UserRegistration
{
    private static UsersLogic _usersLogic = new UsersLogic();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n=== User Registration ===");
        Console.WriteLine("Please fill in the following information:");

        Console.Write("First Name: ");
        string? firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        string? lastName = Console.ReadLine();

        Console.Write("Phone Number: ");
        string? phoneNumber = Console.ReadLine();

        Console.Write("Email Address: ");
        string? emailAddress = Console.ReadLine();

        Console.Write("Password: ");
        string? password = Console.ReadLine();

        Console.Write("Confirm Password: ");
        string? confirmPassword = Console.ReadLine();

        // Validation
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(emailAddress) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            Console.WriteLine("All fields are required!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        if (password != confirmPassword)
        {
            Console.WriteLine("Passwords do not match!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        if (!_usersLogic.IsValidEmail(emailAddress))
        {
            Console.WriteLine("Invalid email format! Please use a valid email address.");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        if (!_usersLogic.IsValidPhoneNumber(phoneNumber))
        {
            Console.WriteLine("Invalid phone number! Must be at least 10 digits.");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
            return;
        }

        // Set default role as customer
        string defaultRole = "customer";

        // Create user account in Users table
        bool userCreated = _usersLogic.CreateUser(firstName, lastName, phoneNumber, emailAddress, password, defaultRole);

        if (userCreated)
        {
            Console.WriteLine("Registration successful! You can now login.");
            Console.WriteLine("Press any key to continue to login...");
            Console.ReadKey();
            UserLogin.Start();
        }
        else
        {
            Console.WriteLine("Registration failed. The email address might already be in use.");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            Start();
        }
    }
}
