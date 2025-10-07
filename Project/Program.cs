// See https://aka.ms/new-console-template for more information

// Initialize database silently
try
{
    DatabaseInitializer.Initialize();
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization failed: {ex.Message}");
    Console.WriteLine("The application may not work correctly.\n");
}

Console.WriteLine("Welcome to Kevin's Fine Dining Restaurant Management System");
Menu.Start();
