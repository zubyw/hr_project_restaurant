// See https://aka.ms/new-console-template for more information

// Initialize database silently
using System.Globalization;

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
 
var culture = new CultureInfo("nl-NL");
 
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
Menu.Start();
