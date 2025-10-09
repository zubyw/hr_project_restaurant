static class UserMakeReservation
{

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("\n===Reservations===");
        Console.WriteLine("1. Make reservation");
        Console.WriteLine("2. View all users");
        Console.WriteLine("3. Search user by email");
        Console.WriteLine("4. View users by role");
        Console.WriteLine("5. Update user");
        Console.WriteLine("6. Delete user");
        Console.WriteLine("7. Back to main menu");

        string? input = Console.ReadLine();
    }
}