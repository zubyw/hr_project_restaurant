using Microsoft.Data.Sqlite;
using Dapper;

public static class DatabaseInitializer
{
    private static readonly string _connectionString = "Data Source=DataSources/project.db;Foreign Keys=False";

    public static void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create Users table if it doesn't exist
        var createUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                PhoneNumber TEXT NOT NULL,
                EmailAddress TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Roles TEXT NOT NULL
            )";
        connection.Execute(createUsersTable);

        // Add Password column if it doesn't exist (for existing databases)
        try
        {
            connection.Execute("ALTER TABLE Users ADD COLUMN Password TEXT");
        }
        catch
        {
            // Column already exists, ignore error
        }

        // Update existing users without password to have a default password
        try
        {
            connection.Execute("UPDATE Users SET Password = 'DefaultPassword123!' WHERE Password IS NULL OR Password = ''");
        }
        catch
        {
            // Ignore any errors
        }

        // Create Tables table
        var createTablesTable = @"
            CREATE TABLE IF NOT EXISTS [Table] (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                TableNumber INTEGER NOT NULL,
                TableCapacity INTEGER NOT NULL,
                IsActive INTEGER NOT NULL DEFAULT 1
            )";
        connection.Execute(createTablesTable);

        // Create Themes table
        var createThemesTable = @"
            CREATE TABLE IF NOT EXISTS Themes (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Course TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1
            )";
        connection.Execute(createThemesTable);

        // Create Dishes table
        var createDishesTable = @"
            CREATE TABLE IF NOT EXISTS Dishes (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Description TEXT,
                Type TEXT NOT NULL
            )";
        connection.Execute(createDishesTable);

        // Create Reservations table
        var createReservationsTable = @"
            CREATE TABLE IF NOT EXISTS Reservations (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                TableId INTEGER NOT NULL,
                GuestCount INTEGER NOT NULL,
                StartAt TEXT NOT NULL,
                Status TEXT NOT NULL,
                CancelledByUserId INTEGER,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(ID),
                FOREIGN KEY (TableId) REFERENCES [Table](ID),
                FOREIGN KEY (CancelledByUserId) REFERENCES Users(ID)
            )";
        connection.Execute(createReservationsTable);

        // Create junction tables
        var createDishesThemesTable = @"
            CREATE TABLE IF NOT EXISTS Dishes_Themes (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                DishId INTEGER NOT NULL,
                ThemeId INTEGER NOT NULL,
                FOREIGN KEY (DishId) REFERENCES Dishes(ID),
                FOREIGN KEY (ThemeId) REFERENCES Themes(ID)
            )";
        connection.Execute(createDishesThemesTable);

        var createReservationsDishesTable = @"
            CREATE TABLE IF NOT EXISTS Reservations_Dishes (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                ReservationId INTEGER NOT NULL,
                DishId INTEGER NOT NULL,
                FOREIGN KEY (ReservationId) REFERENCES Reservations(ID),
                FOREIGN KEY (DishId) REFERENCES Dishes(ID)
            )";
        connection.Execute(createReservationsDishesTable);

        var createThemesCalendarTable = @"
            CREATE TABLE IF NOT EXISTS Themes_Calendar (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                ThemeId INTEGER NOT NULL,
                ForeignKey TEXT,
                Type TEXT,
                TimeSlot TEXT,
                Description TEXT,
                FOREIGN KEY (ThemeId) REFERENCES Themes(ID)
            )";
        connection.Execute(createThemesCalendarTable);

        // Create default admin user if it doesn't exist
        CreateDefaultAdminUser(connection);

        // create default tables if none exist
        CreateDefaultTables(connection);

        // database initialized silently
    }

    private static void CreateDefaultAdminUser(SqliteConnection connection)
    {
        // Check if admin  already exists 
        var existingUser = connection.QueryFirstOrDefault<UserModel>(
            "SELECT * FROM Users WHERE EmailAddress = @Email", 
            new { Email = "admin@gmail.com" });

        if (existingUser == null)
        {
            // Create admin user in Users table
            var adminUser = new
            {
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "0000000000",
                EmailAddress = "admin@gmail.com",
                Password = "Wachtwoord!1",
                Roles = "admin"
            };
            
            connection.Execute(@"
                INSERT INTO Users (FirstName, LastName, PhoneNumber, EmailAddress, Password, Roles) 
                VALUES (@FirstName, @LastName, @PhoneNumber, @EmailAddress, @Password, @Roles)", 
                adminUser);
        }
    }

    private static void CreateDefaultTables(SqliteConnection connection)
    {
        // check if there are already tables
        var existingTables = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM [Table]");

        if (existingTables == 0)
        {
            // make every table in the restaurant
            var tables = new[]
            {
                // 2-persons table
                new { TableNumber = 1, TableCapacity = 2 },
                new { TableNumber = 2, TableCapacity = 2 },
                new { TableNumber = 3, TableCapacity = 2 },
                new { TableNumber = 4, TableCapacity = 2 },

                // 4-person tables  
                new { TableNumber = 5, TableCapacity = 4 },
                new { TableNumber = 6, TableCapacity = 4 },
                new { TableNumber = 7, TableCapacity = 4 },
                new { TableNumber = 8, TableCapacity = 4 },
                new { TableNumber = 9, TableCapacity = 4 },
                new { TableNumber = 10, TableCapacity = 4 },

                // 6-person tables
                new { TableNumber = 11, TableCapacity = 6 },
                new { TableNumber = 12, TableCapacity = 6 },
                new { TableNumber = 13, TableCapacity = 6 },
                new { TableNumber = 14, TableCapacity = 6 }
            };

            foreach (var table in tables)
            {
                connection.Execute(@"
                    INSERT INTO [Table] (TableNumber, TableCapacity, IsActive) 
                    VALUES (@TableNumber, @TableCapacity, 1)", 
                    table);
            }
        }
    }
}
