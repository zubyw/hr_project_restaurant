static class UserManagement
{
    private static UsersLogic _usersLogic = new UsersLogic();

    public static void Start()
    {
        // Check if current user is admin
        if (!IsCurrentUserAdmin())
        {
            Console.WriteLine("Access denied. Admin privileges required.");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
            Menu.ShowMainMenu();
            return;
        }

        Console.Clear();
        Console.WriteLine("\n=== User Management (Admin Only) ===");
        Console.WriteLine("1. Create new user");
        Console.WriteLine("2. View all users");
        Console.WriteLine("3. Search user by email");
        Console.WriteLine("4. View users by role");
        Console.WriteLine("5. Update user");
        Console.WriteLine("6. Delete user");
        Console.WriteLine("7. Back to main menu");

        string? input = Console.ReadLine();
        
        switch (input)
        {
            case "1":
                CreateUser();
                break;
            case "2":
                ViewAllUsers();
                break;
            case "3":
                SearchUserByEmail();
                break;
            case "4":
                ViewUsersByRole();
                break;
            case "5":
                UpdateUser();
                break;
            case "6":
                DeleteUser();
                break;
            case "7":
                Menu.ShowMainMenu();
                return;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                Start();
                break;
        }

        // Return to user management menu after completing an action
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Start();
    }

    private static void CreateUser()
    {
        Console.WriteLine("\n=== Create New User ===");
        
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
        
        Console.Write("Role (admin/staff/customer): ");
        string? roles = Console.ReadLine();

        // Validation
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || 
            string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(emailAddress) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(roles))
        {
            Console.WriteLine("All fields are required!");
            return;
        }

        if (!_usersLogic.IsValidEmail(emailAddress))
        {
            Console.WriteLine("Invalid email format!");
            return;
        }

        if (!_usersLogic.IsValidPhoneNumber(phoneNumber))
        {
            Console.WriteLine("Invalid phone number! Must be at least 10 digits.");
            return;
        }

        bool success = _usersLogic.CreateUser(firstName, lastName, phoneNumber, emailAddress, password, roles);
        
        if (success)
        {
            Console.WriteLine("User created successfully!");
        }
        else
        {
            Console.WriteLine("Failed to create user. Email might already exist.");
        }
    }

    private static void ViewAllUsers()
    {
        Console.WriteLine("\n=== All Users ===");
        var users = _usersLogic.GetAllUsers();
        
        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (var user in users)
        {
            DisplayUser(user);
            Console.WriteLine("---");
        }
    }

    private static void SearchUserByEmail()
    {
        Console.WriteLine("\n=== Search User by Email ===");
        Console.Write("Enter email address: ");
        string? email = Console.ReadLine();

        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Email is required!");
            return;
        }

        var user = _usersLogic.GetUserByEmail(email);
        
        if (user != null)
        {
            DisplayUser(user);
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }

    private static void ViewUsersByRole()
    {
        Console.WriteLine("\n=== View Users by Role ===");
        Console.Write("Enter role (admin/staff/customer): ");
        string? role = Console.ReadLine();

        if (string.IsNullOrEmpty(role))
        {
            Console.WriteLine("Role is required!");
            return;
        }

        var users = _usersLogic.GetUsersByRole(role);
        
        if (users.Count == 0)
        {
            Console.WriteLine($"No users found with role: {role}");
            return;
        }

        foreach (var user in users)
        {
            DisplayUser(user);
            Console.WriteLine("---");
        }
    }

    private static void UpdateUser()
    {
        Console.WriteLine("\n=== Update User ===");
        Console.Write("Enter user ID to update: ");
        string? idInput = Console.ReadLine();

        if (!int.TryParse(idInput, out int userId))
        {
            Console.WriteLine("Invalid user ID!");
            return;
        }

        var user = _usersLogic.GetUserById(userId);
        if (user == null)
        {
            Console.WriteLine("User not found!");
            return;
        }

        Console.WriteLine("Current user details:");
        DisplayUser(user);

        Console.WriteLine("\nEnter new details (press Enter to keep current value):");
        
        Console.Write($"First Name ({user.FirstName}): ");
        string? firstName = Console.ReadLine();
        if (!string.IsNullOrEmpty(firstName)) user.FirstName = firstName;

        Console.Write($"Last Name ({user.LastName}): ");
        string? lastName = Console.ReadLine();
        if (!string.IsNullOrEmpty(lastName)) user.LastName = lastName;

        Console.Write($"Phone Number ({user.PhoneNumber}): ");
        string? phoneNumber = Console.ReadLine();
        if (!string.IsNullOrEmpty(phoneNumber)) user.PhoneNumber = phoneNumber;

        Console.Write($"Email Address ({user.EmailAddress}): ");
        string? emailAddress = Console.ReadLine();
        if (!string.IsNullOrEmpty(emailAddress)) user.EmailAddress = emailAddress;

        Console.Write($"Role ({user.Roles}): ");
        string? roles = Console.ReadLine();
        if (!string.IsNullOrEmpty(roles)) user.Roles = roles;

        bool success = _usersLogic.UpdateUser(user);
        
        if (success)
        {
            Console.WriteLine("User updated successfully!");
        }
        else
        {
            Console.WriteLine("Failed to update user.");
        }
    }

    private static void DeleteUser()
    {
        Console.WriteLine("\n=== Delete User ===");
        Console.Write("Enter user ID to delete: ");
        string? idInput = Console.ReadLine();

        if (!int.TryParse(idInput, out int userId))
        {
            Console.WriteLine("Invalid user ID!");
            return;
        }

        var user = _usersLogic.GetUserById(userId);
        if (user == null)
        {
            Console.WriteLine("User not found!");
            return;
        }

        Console.WriteLine("User to delete:");
        DisplayUser(user);

        Console.Write("Are you sure you want to delete this user? (y/N): ");
        string? confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            bool success = _usersLogic.DeleteUser(userId);
            
            if (success)
            {
                Console.WriteLine("User deleted successfully!");
            }
            else
            {
                Console.WriteLine("Failed to delete user.");
            }
        }
        else
        {
            Console.WriteLine("Delete cancelled.");
        }
    }

    private static void DisplayUser(UserModel user)
    {
        Console.WriteLine($"ID: {user.ID}");
        Console.WriteLine($"Name: {user.FirstName} {user.LastName}");
        Console.WriteLine($"Phone: {user.PhoneNumber}");
        Console.WriteLine($"Email: {user.EmailAddress}");
        Console.WriteLine($"Role: {user.Roles}");
    }

    // Helper method to check if current user is admin
    private static bool IsCurrentUserAdmin()
    {
        if (Menu.CurrentUser == null) return false;
        
        // Check if user is admin by email or by getting user role from Users table
        if (Menu.CurrentUser.EmailAddress == "admin@gmail.com") return true;
        
        try
        {
            var usersAccess = new UsersAccess();
            var user = usersAccess.GetByEmail(Menu.CurrentUser.EmailAddress);
            return user?.Roles?.Contains("admin") == true;
        }
        catch
        {
            return false;
        }
    }
}
