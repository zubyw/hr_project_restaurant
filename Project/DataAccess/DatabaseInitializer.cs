using Microsoft.Data.Sqlite;
using Dapper;

public static class DatabaseInitializer
{
    private static readonly string _connectionString = "Data Source=DataSources/project.db";

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

        // Create default themes and dishes if none exist
        CreateDefaultThemesAndDishes(connection);

        // database initialized silently
    }

    private static void CreateDefaultAdminUser(SqliteConnection connection)
    {
        // Check if admin already exists 
        var existingUser = connection.QueryFirstOrDefault<UserModel>(
            "SELECT * FROM Users WHERE EmailAddress = @Email", 
            new { Email = "admin" });

        if (existingUser == null)
        {
            // Create admin user in Users table
            var adminUser = new
            {
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "0000000000",
                EmailAddress = "admin",
                Password = "admin",
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

    private static void CreateDefaultThemesAndDishes(SqliteConnection connection)
    {
        // Check if there are already dishes
        var existingDishes = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM Dishes");

        if (existingDishes == 0)
        {
            // Create Japanese Theme
            var japaneseTheme = new
            {
                Name = "Japanese",
                Course = "Authentic Japanese Cuisine",
                IsActive = 1
            };
            connection.Execute(@"
                INSERT INTO Themes (Name, Course, IsActive) 
                VALUES (@Name, @Course, @IsActive)", 
                japaneseTheme);
            int japaneseThemeId = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");

            // Create Italian Theme
            var italianTheme = new
            {
                Name = "Italian",
                Course = "Traditional Italian Flavors",
                IsActive = 1
            };
            connection.Execute(@"
                INSERT INTO Themes (Name, Course, IsActive) 
                VALUES (@Name, @Course, @IsActive)", 
                italianTheme);
            int italianThemeId = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");

            // Create French Theme
            var frenchTheme = new
            {
                Name = "French",
                Course = "Classic French Cuisine",
                IsActive = 1
            };
            connection.Execute(@"
                INSERT INTO Themes (Name, Course, IsActive) 
                VALUES (@Name, @Course, @IsActive)", 
                frenchTheme);
            int frenchThemeId = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");

            // ========== JAPANESE DISHES ==========
            
            // Japanese Starters
            var japaneseDishes = new[]
            {
                new { Name = "Edamame", Price = 6.50m, Description = "Steamed soybeans with sea salt", Type = "Starter", ThemeId = japaneseThemeId },
                new { Name = "Miso Soup", Price = 5.50m, Description = "Traditional soup with tofu and seaweed", Type = "Starter", ThemeId = japaneseThemeId },
                new { Name = "Gyoza", Price = 8.50m, Description = "Pan-fried dumplings with soy dipping sauce", Type = "Starter", ThemeId = japaneseThemeId },
                new { Name = "Sushi Platter", Price = 12.50m, Description = "Assorted nigiri and maki rolls", Type = "Starter", ThemeId = japaneseThemeId },
                
                // Japanese Mains
                new { Name = "Ramen", Price = 16.50m, Description = "Rich tonkotsu broth with noodles and pork", Type = "Main", ThemeId = japaneseThemeId },
                new { Name = "Teriyaki Chicken", Price = 18.50m, Description = "Grilled chicken with teriyaki glaze", Type = "Main", ThemeId = japaneseThemeId },
                new { Name = "Beef Yakisoba", Price = 19.50m, Description = "Stir-fried noodles with beef and vegetables", Type = "Main", ThemeId = japaneseThemeId },
                new { Name = "Salmon Sashimi", Price = 22.50m, Description = "Fresh salmon slices with wasabi and soy", Type = "Main", ThemeId = japaneseThemeId },
                
                // Japanese Desserts
                new { Name = "Mochi Ice Cream", Price = 7.50m, Description = "Rice cake filled with ice cream", Type = "Dessert", ThemeId = japaneseThemeId },
                new { Name = "Dorayaki", Price = 6.50m, Description = "Sweet red bean pancake sandwich", Type = "Dessert", ThemeId = japaneseThemeId },
                new { Name = "Matcha Tiramisu", Price = 8.50m, Description = "Japanese twist on Italian classic", Type = "Dessert", ThemeId = japaneseThemeId },
                new { Name = "Yuzu Cheesecake", Price = 9.50m, Description = "Light cheesecake with citrus flavor", Type = "Dessert", ThemeId = japaneseThemeId }
            };

            // ========== ITALIAN DISHES ==========
            
            var italianDishes = new[]
            {
                // Italian Starters
                new { Name = "Bruschetta", Price = 7.50m, Description = "Toasted bread with tomatoes and basil", Type = "Starter", ThemeId = italianThemeId },
                new { Name = "Caprese Salad", Price = 9.50m, Description = "Fresh mozzarella, tomatoes, and basil", Type = "Starter", ThemeId = italianThemeId },
                new { Name = "Arancini", Price = 8.50m, Description = "Fried risotto balls with mozzarella", Type = "Starter", ThemeId = italianThemeId },
                new { Name = "Carpaccio", Price = 12.50m, Description = "Thinly sliced raw beef with arugula", Type = "Starter", ThemeId = italianThemeId },
                
                // Italian Mains
                new { Name = "Spaghetti Carbonara", Price = 16.50m, Description = "Pasta with eggs, bacon, and pecorino", Type = "Main", ThemeId = italianThemeId },
                new { Name = "Margherita Pizza", Price = 14.50m, Description = "Classic pizza with tomato and mozzarella", Type = "Main", ThemeId = italianThemeId },
                new { Name = "Osso Buco", Price = 24.50m, Description = "Braised veal shanks with gremolata", Type = "Main", ThemeId = italianThemeId },
                new { Name = "Risotto ai Funghi", Price = 18.50m, Description = "Creamy mushroom risotto", Type = "Main", ThemeId = italianThemeId },
                
                // Italian Desserts
                new { Name = "Tiramisu", Price = 8.50m, Description = "Coffee-soaked ladyfingers with mascarpone", Type = "Dessert", ThemeId = italianThemeId },
                new { Name = "Panna Cotta", Price = 7.50m, Description = "Vanilla cream with berry sauce", Type = "Dessert", ThemeId = italianThemeId },
                new { Name = "Cannoli", Price = 7.50m, Description = "Crispy tubes filled with sweet ricotta", Type = "Dessert", ThemeId = italianThemeId },
                new { Name = "Gelato", Price = 6.50m, Description = "Italian ice cream, three flavors", Type = "Dessert", ThemeId = italianThemeId }
            };

            // ========== FRENCH DISHES ==========
            
            var frenchDishes = new[]
            {
                // French Starters
                new { Name = "French Onion Soup", Price = 8.50m, Description = "Rich soup with caramelized onions and cheese", Type = "Starter", ThemeId = frenchThemeId },
                new { Name = "Escargots", Price = 12.50m, Description = "Snails in garlic butter", Type = "Starter", ThemeId = frenchThemeId },
                new { Name = "Pâté de Campagne", Price = 10.50m, Description = "Country-style terrine with cornichons", Type = "Starter", ThemeId = frenchThemeId },
                new { Name = "Salade Niçoise", Price = 11.50m, Description = "Salad with tuna, eggs, and olives", Type = "Starter", ThemeId = frenchThemeId },
                
                // French Mains
                new { Name = "Coq au Vin", Price = 22.50m, Description = "Chicken braised in red wine", Type = "Main", ThemeId = frenchThemeId },
                new { Name = "Boeuf Bourguignon", Price = 24.50m, Description = "Beef stew in burgundy wine", Type = "Main", ThemeId = frenchThemeId },
                new { Name = "Duck Confit", Price = 26.50m, Description = "Slow-cooked duck leg with potatoes", Type = "Main", ThemeId = frenchThemeId },
                new { Name = "Ratatouille", Price = 16.50m, Description = "Provençal vegetable stew", Type = "Main", ThemeId = frenchThemeId },
                
                // French Desserts
                new { Name = "Crème Brûlée", Price = 9.50m, Description = "Vanilla custard with caramelized sugar", Type = "Dessert", ThemeId = frenchThemeId },
                new { Name = "Tarte Tatin", Price = 8.50m, Description = "Upside-down caramelized apple tart", Type = "Dessert", ThemeId = frenchThemeId },
                new { Name = "Profiteroles", Price = 9.50m, Description = "Cream puffs with chocolate sauce", Type = "Dessert", ThemeId = frenchThemeId },
                new { Name = "Macarons", Price = 7.50m, Description = "Assorted French meringue cookies", Type = "Dessert", ThemeId = frenchThemeId }
            };

            // Insert all dishes
            var allDishes = japaneseDishes.Concat(italianDishes).Concat(frenchDishes);
            
            foreach (var dish in allDishes)
            {
                // Insert dish
                connection.Execute(@"
                    INSERT INTO Dishes (Name, Price, Description, Type) 
                    VALUES (@Name, @Price, @Description, @Type)", 
                    dish);
                
                int dishId = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");
                
                // Link dish to theme
                connection.Execute(@"
                    INSERT INTO Dishes_Themes (DishId, ThemeId) 
                    VALUES (@DishId, @ThemeId)", 
                    new { DishId = dishId, ThemeId = dish.ThemeId });
            }

            // Set current month's theme (Japanese for now - you can change this logic)
            var currentMonth = DateTime.Now.Month;
            int currentThemeId = currentMonth % 3 == 1 ? japaneseThemeId : 
                                 currentMonth % 3 == 2 ? italianThemeId : 
                                 frenchThemeId;

            // Add to calendar for current month
            connection.Execute(@"
                INSERT INTO Themes_Calendar (ThemeId, Type, TimeSlot, Description) 
                VALUES (@ThemeId, 'Monthly', @TimeSlot, @Description)", 
                new { 
                    ThemeId = currentThemeId, 
                    TimeSlot = DateTime.Now.ToString("yyyy-MM-01"),
                    Description = "Current month theme"
                });
        }
    }
}
