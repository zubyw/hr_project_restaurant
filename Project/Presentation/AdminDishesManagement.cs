using System;
using System.Collections.Generic;
using Project.Logic;
using Project.DataModels;
using Project.DataAccess;
using System.Runtime.InteropServices;

public static class AdminDishesManagement
{
    private static readonly DishLogic _logic = new DishLogic();
    private static readonly AllergenAccess _allergenAccess = new AllergenAccess();

    public static void Start()
    {
        string[] options =
        {
            "Create new dish",
            "Manage all dishes",
            "Link drink to main dish",
            "View linked drinks",
            "Back"
        };
        bool choosing = true;
        while (choosing)
        {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Dishes Management ===");
            switch (index)
            {
                case 0:
                    Add();
                    break;

                case 1:
                    ManageAllDishes();
                    break;

                case 2:
                    LinkDrinkToMainDish();
                    break;
                case 3:

                    ViewLinkedDrinks();
                    break;

                case 4:
                    choosing = false;
                    Menu.ShowAdminMenu();
                    break;
            }
        
        }
    }

    private static void Add()
    {
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        
        string name = CreateDishName();
        decimal price = CreateDishPrice();
        string description = CreateDishDescription();
        string type = CreateDishGiveType();
        List<int> selectedAllergenIds = SelectAllergens();
        
        DishModel dish = new DishModel(){Name = name, Price = price, Description = description, Type = type}; 
        int dishId = _logic.WriteIntoDBAndReturnId(dish);
        
        // Link allergens to dish
        foreach (int allergenId in selectedAllergenIds)
        {
            _allergenAccess.LinkDishToAllergen(dishId, allergenId);
        }

        Console.WriteLine("Dish created.");
        Thread.Sleep(1500);
    }
    private static string CreateDishName()
    {
        Console.Write("Name: ");
        string? name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            Thread.Sleep(1500);
            return CreateDishName(); // recursion until valid string for the name is given
        }

        if (_logic.DoesDishExist(name))
        {
            Console.WriteLine("There already is a dish with this name.");
            Thread.Sleep(1500);
            Start();
        }

        return name;
    }

    private static decimal CreateDishPrice()
    {
        Console.Write("Price (example: 12,50): ");
        string? priceInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(priceInput))
        {
            Console.WriteLine("Price cannot be empty.");
            Thread.Sleep(1500);
            return CreateDishPrice();
        }

        if (priceInput.Contains("."))
        {
            Console.WriteLine("Please use a comma ',' as the decimal separator (example 12,50).");
            Thread.Sleep(1500);
            return CreateDishPrice();
        }

        try
        {
            return decimal.Parse(priceInput);
        }
        catch
        {
            Console.WriteLine("Invalid price format.");
            Thread.Sleep(1500);
            return CreateDishPrice(); // same as CreateDishName Recursion so that the price will be valid
        }
    }

    private static string CreateDishDescription()
    {
        Console.Write("Description: ");
        string? description = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description cannot be empty.");
            Thread.Sleep(1500);
            return CreateDishDescription(); // again recursion until valid
        }
        return description;
    }

    private static string CreateDishGiveType()
    {
        string[] types = { "Starter", "Main", "Dessert" };
        int typeIndex = MenuHelper.ShowMenuUpDown(types, "Type (Starter/Main/Dessert): ");
        return types[typeIndex];
    }

    private static List<int> SelectAllergens()
    {
        List<AllergenModel> allergens = _allergenAccess.GetAll();
        List<bool> selectedStates = new List<bool>();
        
        for (int i = 0; i < allergens.Count; i++)
        {
            selectedStates.Add(false);
        }
        List<int> selectedIds = new List<int>();
        int currentIndex = 0;
        bool selecting = true;
        while (selecting)
        {
            Console.Clear();
            Console.WriteLine("=== Select allergens ===");
            Console.WriteLine();

            for (int i = 0; i < allergens.Count; i++)
            {
                bool isSelected = i == currentIndex;
                bool isChecked = selectedStates[i];

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                string checkbox = isChecked ? "[X]" : "[ ]";
                Console.WriteLine($"{checkbox} {allergens[i].Name}");

                if (isSelected)
                {
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press SPACE to select | ENTER to confirm");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    currentIndex = (currentIndex - 1 + allergens.Count) % allergens.Count;
                    break;

                case ConsoleKey.DownArrow:
                    currentIndex = (currentIndex + 1) % allergens.Count;
                    break;

                case ConsoleKey.Spacebar:
                    selectedStates[currentIndex] = !selectedStates[currentIndex];
                    break;

                case ConsoleKey.Enter:
                    for (int i = 0; i < allergens.Count; i++)
                    {
                        if (selectedStates[i])
                        {
                            selectedIds.Add(allergens[i].ID);
                        }
                    }
                    selecting = false;
                    break;
            }
        }
        return selectedIds;
    }

    private static void ManageAllDishes()
    {
        List<DishModel> dishes = _logic.GetAllDishes();

        if (dishes.Count == 0)
        {
            Console.WriteLine("No dishes found.");
            Console.ReadKey();
            return;
        }

        int index = 0;
        bool managing = true;
        while (managing)
        {
            Console.Clear();
            Console.WriteLine("=== All Dishes ===\n");

            Console.WriteLine("┌────────────────────────┬───────────────┬───────────┐");
            Console.WriteLine("│ Name                   │ Type          │ Price     │");
            Console.WriteLine("├────────────────────────┼───────────────┼───────────┤");

            for (int i = 0; i < dishes.Count; i++)
            {
                DishModel dish = dishes[i];

                string name = dish.Name.Length > 20 ? dish.Name[..17] + "..." : dish.Name;
                string type = dish.Type.Length > 12 ? dish.Type[..9] + "..." : dish.Type;

                bool selected = (i == index);

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine("│ {0,-22} │ {1,-13} │ {2,9:F2} │",
                    name, type, dish.Price);

                if (selected)
                    Console.ResetColor();
            }

            Console.WriteLine("└────────────────────────┴───────────────┴───────────┘");

            Console.WriteLine("\n ↑↓ Select dish | ESC = back");

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow)
            {
                index = (index + 1) % dishes.Count;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                index = (index - 1 + dishes.Count) % dishes.Count;
            }
            else if (key == ConsoleKey.Enter)
            {
                DishModel selectedDish = dishes[index];
                ManageDish(selectedDish);
                index = 0;
                dishes = _logic.GetAllDishes();
            }
            else if (key == ConsoleKey.Escape)
            {
                managing = false;
            }
        }
        Start();
    }

    private static void ManageDish(DishModel dish)
    {
        string[] options =
        {
            "Edit dish",
            "Delete dish",
            "Back"
        };
        bool managingDish = true;
        while (managingDish)
        {
            int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Manage dish ===");
        
            switch (index)
            {
                case 0:
                    EditDish(dish);
                    break;

                case 1:
                    Delete(dish);
                    managingDish = false;
                    break;

                case 2:
                    managingDish = false;
                    break;
            }
        }
    }


    private static void EditDish(DishModel dish)
    {
        string[] options =
        {
            "Edit Name",
            "Edit Price",
            "Edit Description",
            "Edit Type",
            "Edit Allergens",
            "Back"
        };
        bool editingDish = true;
        while (editingDish)
        {
        int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage dish ===\n\n{dish.ToString()}");
        try
        {
            switch (index)
            {
                case 0:
                    EditDishName(dish);
                    break;

                case 1:
                    EditDishPrice(dish);
                    break;

                case 2:
                    EditDishDescription(dish);
                    break;
                case 3:
                    EditDishType(dish);
                    break;
                case 4:
                    ManageAllergens(dish);
                    break;
                case 5:
                    editingDish = false;
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
        }
    }
    }

    public static void EditDishName(DishModel dish)
    {
        Console.Clear();
        Console.WriteLine($"Current name: {dish.Name}");
        Console.WriteLine("New name:");

        string? newName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(newName))
        {
            Console.WriteLine("Name cannot be empty.");
            Thread.Sleep(1500);
            return; // the input is empty sp return 
        }

        dish.Name = newName;
        _logic.UpdateDish(dish);

        Console.WriteLine("Name updated successfully.");
        Thread.Sleep(1500);
    }
    public static void EditDishPrice(DishModel dish)
    {
        Console.Clear();
        Console.WriteLine($"Current price: {dish.Price}");
        Console.WriteLine("New price (example: 12,50):");

        string? newPriceInput = Console.ReadLine();

        if (newPriceInput.Contains("."))
        {
            Console.WriteLine("Please use a comma ',' as the decimal separator (example 12,50).");
            Thread.Sleep(1500);
            return;
        }

        try
        {
            decimal price = decimal.Parse(newPriceInput);

            dish.Price = price;
            _logic.UpdateDish(dish);

            Console.WriteLine("Price updated successfully.");
            Thread.Sleep(1500);
        }
        catch (Exception)
        {
            Console.WriteLine("Invalid price format.");
            Thread.Sleep(1500);
        }
    }
    public static void EditDishDescription(DishModel dish)
    {
        Console.Clear();
        Console.WriteLine($"Current description: {dish.Description}");
        Console.WriteLine("New description:");

        string? newDescription = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(newDescription))
        {
            Console.WriteLine("Description cannot be empty.");
            Thread.Sleep(1500);
            return; // exit if invalid
        }

        dish.Description = newDescription;
        _logic.UpdateDish(dish);

        Console.WriteLine("Description updated successfully.");
        Thread.Sleep(1500);
    }
    public static void EditDishType(DishModel dish)
    {
        string[] types = { "Starter", "Main", "Dessert" };

        int typeIndex = MenuHelper.ShowMenuUpDown(types, $"Current type: {dish.Type}");
        string selectedType = types[typeIndex];

        if (selectedType == dish.Type)
        {
            Console.WriteLine($"{selectedType} is already this dish's type.");
            Thread.Sleep(1500);
            return; // exit if no change
        }

        dish.Type = selectedType;
        _logic.UpdateDish(dish);

        Console.WriteLine("Dish type updated successfully.");
        Thread.Sleep(1500);
    }





    private static void ManageAllergens(DishModel dish)
    {
        List<AllergenModel> allergens = _allergenAccess.GetAll();
        List<int> currentAllergenIds = _allergenAccess.GetAllergenIdsByDishId(dish.ID);
        List<bool> selectedStates = new List<bool>();
        
        for (int i = 0; i < allergens.Count; i++)
        {
            selectedStates.Add(currentAllergenIds.Contains(allergens[i].ID));
        }

        int currentIndex = 0;
        bool managingAllergens = true;
        while (managingAllergens)
        {
            Console.Clear();
            Console.WriteLine();

            for (int i = 0; i < allergens.Count; i++)
            {
                bool isSelected = i == currentIndex;
                bool isChecked = selectedStates[i];

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                string checkbox = isChecked ? "[X]" : "[ ]";
                Console.WriteLine($"{checkbox} {allergens[i].Name}");

                if (isSelected)
                {
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press SPACE to select | ENTER to save | ESC to cancel");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    currentIndex = (currentIndex - 1 + allergens.Count) % allergens.Count;
                    break;

                case ConsoleKey.DownArrow:
                    currentIndex = (currentIndex + 1) % allergens.Count;
                    break;

                case ConsoleKey.Spacebar:
                    selectedStates[currentIndex] = !selectedStates[currentIndex];
                    break;

                case ConsoleKey.Enter:
                    _allergenAccess.UnlinkAllAllergensFromDish(dish.ID);
                    for (int i = 0; i < allergens.Count; i++)
                    {
                        if (selectedStates[i])
                        {
                            _allergenAccess.LinkDishToAllergen(dish.ID, allergens[i].ID);
                        }
                    }
                    
                    dish.AllergenIds = _allergenAccess.GetAllergenIdsByDishId(dish.ID);
                    dish.AllergenNames = _allergenAccess.GetAllergensByDishId(dish.ID).Select(a => a.Name).ToList();
                    
                    Console.WriteLine("\nAllergens updated successfully!");
                    Thread.Sleep(1500);
                    managingAllergens = false;
                    break;

                case ConsoleKey.Escape:
                    managingAllergens = false;
                    break;
            }
        }
    }

    private static void Delete(DishModel dish)
    {
            string[] options =
            {
                "Yes",
                "No"
            };
            bool deleting = true;
            while (deleting)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: delete dish ===\n\n{dish.ToString()}\n\nDelete dish?");
                switch (index)
                {
                    case 0: // remove dish from: Dishes, Dishes_Themes, Reservation_Dishes
                        _logic.DeleteDishInDishes_Themes(dish);
                        _logic.DeleteDishInReservations_Dishes(dish);
                        _logic.DeleteDishInDishes(dish);
                        Console.WriteLine($"Dish: {dish.Name} deleted");
                        Thread.Sleep(1500);
                        ManageAllDishes();
                        deleting = false;
                        break;
                    case 1:
                        deleting = false;
                        break;
                }
        }
    }

    
    private static void LinkDrinkToMainDish()
{
    DishLogic dishLogic = new DishLogic();
    DrinkLogic drinkLogic = new DrinkLogic();

    bool linking = true;

    while (linking)
    {
        List<DishModel> dishes = dishLogic.GetAllDishes()
            .Where(d => d.Type == "Main")
            .ToList();

        if (dishes.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No main dishes available.");
            Console.ReadKey();
            return;
        }
// selecting dish
        string[] dishOptions = dishes.Select(d => $"{d.Name,-30} €{d.Price,6:F2}").ToArray();
        int dishIndex = MenuHelper.ShowMenuUpDown(dishOptions, "=== Link drink to main dish ===");
        DishModel selectedDish = dishes[dishIndex];

        // Get drinks
        List<Drink> drinks = drinkLogic.GetAllDrinks();
        if (drinks.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No drinks available.");
            Console.ReadKey();
            return;
        }

// selecting drink
        string[] drinkOptions = drinks
            .Select(d => $"{d.Name,-25} {d.AlcoholPercentage,5:F1}%  €{d.Price,6:F2}")
            .ToArray();
        int drinkIndex = MenuHelper.ShowMenuUpDown(drinkOptions, "=== Select drink ===");
        Drink selectedDrink = drinks[drinkIndex];

        // Link drink to dish
        dishLogic.LinkDrinkToMainDish(selectedDish.ID, selectedDrink.ID);

        Console.Clear();
        Console.WriteLine("Drink linked successfully!\n");
        Console.WriteLine($"Dish : {selectedDish.Name}");
        Console.WriteLine($"Drink: {selectedDrink.Name}");
        Console.WriteLine("\nPress any key to link another drink or ESC to return...");

        var key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Escape)
            linking = false;
    }
}


    private static void ViewLinkedDrinks()
    {
        DishLogic dishLogic = new DishLogic();
        DrinkLogic drinkLogic = new DrinkLogic();

        // ALTIJD opnieuw ophalen
        List<DishModel> dishes = dishLogic.GetAllDishes();

        Console.Clear();
        Console.WriteLine("=== Main Dishes & Linked Drinks ===\n");

        Console.WriteLine("┌────────────────────────┬────────────────────────────┬───────────┐");
        Console.WriteLine("│ Dish                   │ Drink                      │ Price     │");
        Console.WriteLine("├────────────────────────┼────────────────────────────┼───────────┤");

        foreach (DishModel dish in dishes)
        {
            if (dish.Type != "Main")
                continue;

            Drink linkedDrink = drinkLogic.GetDrinkForDish(dish.ID);

            string dishName = dish.Name.Length > 22
                ? dish.Name.Substring(0, 19) + "..."
                : dish.Name;

            if (linkedDrink != null)
            {
                string drinkName = linkedDrink.Name.Length > 26
                    ? linkedDrink.Name.Substring(0, 23) + "..."
                    : linkedDrink.Name;

                Console.WriteLine(
                    "│ {0,-22} │ {1,-26} │ {2,9:F2} │",
                    dishName,
                    drinkName,
                    linkedDrink.Price
                );
            }
            else
            {
                Console.WriteLine(
                    "│ {0,-22} │ {1,-26} │ {2,9} │",
                    dishName,
                    "No drink linked",
                    "-"
                );
            }
        }

        Console.WriteLine("└────────────────────────┴────────────────────────────┴───────────┘");
        Console.WriteLine("\nESC = back");
        Console.ReadKey(true);
    }
}

    