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

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Dishes Management ===");
        try
        {
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
                    Menu.ShowAdminMenu();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
        }
    }
    }

    private static void Add()
    {
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        
        Console.Write("Name: ");
        string name = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(name))
        {
        Console.WriteLine("Name cannot be empty.");
        Add();
        }
        if(_logic.DoesDishExist(name))
        {
            Console.WriteLine("There already is a dish with this name");
            Thread.Sleep(1500);
            Start();
        }
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        Console.Write("Price (example: 12.50): ");
        string priceInput = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(priceInput))
        {
        Console.WriteLine("Price cannot be empty.");
        Add();
        }
        decimal price = decimal.Parse(priceInput.Replace(',', '.'),System.Globalization.CultureInfo.InvariantCulture);
        
        Console.Clear();
        Console.WriteLine("=== Create new dish ===");
        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(description))
        {
        Console.WriteLine("Description cannot be empty.");
        Add();
        }


        Console.Clear();
        Console.WriteLine("=== Create new dish ===\nType (Starter/Main/Dessert): ");
        string[] types = { "Starter", "Main", "Dessert" };
        int typeIndex = MenuHelper.ShowMenuUpDown(types, "=== Create new dish ===\nType (Starter/Main/Dessert): ");
        string type = types[typeIndex];
        
        // Select allergens
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

    private static List<int> SelectAllergens()
    {
        List<AllergenModel> allergens = _allergenAccess.GetAll();
        List<bool> selectedStates = new List<bool>();
        
        for (int i = 0; i < allergens.Count; i++)
        {
            selectedStates.Add(false);
        }

        int currentIndex = 0;

        while (true)
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
                    List<int> selectedIds = new List<int>();
                    for (int i = 0; i < allergens.Count; i++)
                    {
                        if (selectedStates[i])
                        {
                            selectedIds.Add(allergens[i].ID);
                        }
                    }
                    return selectedIds;
            }
        }
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

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== All Dishes ===\n");

            Console.WriteLine("┌────────────────────────┬───────────────┬───────────┐");
            Console.WriteLine("│ Name                   │ Type          │ Price     │");
            Console.WriteLine("├────────────────────────┼───────────────┼───────────┤");

            for (int i = 0; i < dishes.Count; i++)
            {
                var dish = dishes[i];

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
            }
            else if (key == ConsoleKey.Escape)
            {
                Start();
            }
        }
    }

    private static void ManageDish(DishModel dish)
    {
        string[] options =
        {
            "Edit dish",
            "Delete dish",
            "Back"
        };

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, "=== Admin: Manage dish ===");
        try
        {
            switch (index)
            {
                case 0:
                    EditDish(dish);
                    return;

                case 1:
                    Delete(dish);
                    break;

                case 2:
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

        while (true)
    {
        int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: Manage dish ===\n\n{dish.ToString()}");
        try
        {
            switch (index)
            {
                case 0:
                    Console.Clear();
                    Console.WriteLine($"Name: {dish.Name}");
                    Console.WriteLine();
                    string newname = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(newname))
                    {
                    Console.WriteLine("Name cannot be empty.");
                    Thread.Sleep(1500);
                    continue;
                    }
                    dish.Name = newname;
                    _logic.UpdateDish(dish);
                    break;

                case 1:
                    Console.Clear();
                    Console.WriteLine($"Price: {dish.Price}");
                    Console.WriteLine();
                    string newprice = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(newprice))
                    {
                    Console.WriteLine("Price cannot be empty.");
                    Thread.Sleep(1500);
                    continue;
                    }
                    decimal price = decimal.Parse(newprice.Replace(',', '.'),System.Globalization.CultureInfo.InvariantCulture);
                    dish.Price = price;
                    _logic.UpdateDish(dish);
                    break;

                case 2:
                    Console.Clear();
                    Console.WriteLine($"Description: {dish.Description}");
                    Console.WriteLine();
                    string newDescription = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(newDescription))
                    {
                    Console.WriteLine("Description cannot be empty.");
                    Thread.Sleep(1500);
                    continue;
                    }
                    dish.Description = newDescription;
                    _logic.UpdateDish(dish);
                    break;
                case 3:
                    string[] types = { "Starter", "Main", "Dessert" };
                    int typeIndex = MenuHelper.ShowMenuUpDown(types, $"Type: {dish.Type}");
                    string type = types[typeIndex];
                    if (type == dish.Type)
                        {
                            Console.WriteLine($"{type} is already this dish's type");
                            Thread.Sleep(1500);
                            continue;
                        }
                    dish.Type = type;
                    _logic.UpdateDish(dish);
                    break;
                case 4:
                    ManageAllergens(dish);
                    break;
                case 5:
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

        while (true)
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
                    return;

                case ConsoleKey.Escape:
                    return;
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

            while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(options, $"=== Admin: delete dish ===\n\n{dish.ToString()}\n\nDelete dish?");
            try
            {
                switch (index)
                {
                    case 0: // remove dish from: Dishes, Dishes_Themes, Reservation_Dishes
                        _logic.DeleteDishInDishes_Themes(dish);
                        _logic.DeleteDishInReservations_Dishes(dish);
                        _logic.DeleteDishInDishes(dish);
                        Console.WriteLine($"Dish: {dish.Name} deleted");
                        Thread.Sleep(1500);
                        ManageAllDishes();
                        return;
                    case 1:
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

    
    private static void LinkDrinkToMainDish()
    {
        DishLogic dishLogic = new DishLogic();
        DrinkLogic drinkLogic = new DrinkLogic();

        List<DishModel> allDishes = dishLogic.GetAllDishes();
        List<DishModel> dishes = new List<DishModel>();

        foreach (DishModel dish in allDishes)
        {
            if (dish.Type == "Main")
            {
                dishes.Add(dish);
            }
        }

        if (dishes.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No main dishes without a linked drink.");
            Console.ReadKey();
            return;
        }

        int dishIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Link drink to main dish ===\n");
            Console.WriteLine("↑ ↓ Select dish | ENTER = choose | ESC = back\n");

            for (int i = 0; i < dishes.Count; i++)
            {
                if (i == dishIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"{dishes[i].Name,-30} €{dishes[i].Price,6:F2}");

                if (i == dishIndex)
                    Console.ResetColor();
            }

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
                dishIndex = (dishIndex - 1 + dishes.Count) % dishes.Count;
            else if (key == ConsoleKey.DownArrow)
                dishIndex = (dishIndex + 1) % dishes.Count;
            else if (key == ConsoleKey.Escape)
                return;
            else if (key == ConsoleKey.Enter)
                break;
        }

        DishModel selectedDish = dishes[dishIndex];

        List<Drink> drinks = drinkLogic.GetAllDrinks();
        if (drinks.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No drinks available.");
            Console.ReadKey();
            return;
        }

        int drinkIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Select drink ===\n");
            Console.WriteLine("↑ ↓ Select drink | ENTER = link | ESC = back\n");

            for (int i = 0; i < drinks.Count; i++)
            {
                if (i == drinkIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(
                    $"{drinks[i].Name,-25} {drinks[i].AlcoholPercentage,5:F1}%  €{drinks[i].Price,6:F2}"
                );

                if (i == drinkIndex)
                    Console.ResetColor();
            }

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
                drinkIndex = (drinkIndex - 1 + drinks.Count) % drinks.Count;
            else if (key == ConsoleKey.DownArrow)
                drinkIndex = (drinkIndex + 1) % drinks.Count;
            else if (key == ConsoleKey.Escape)
                return;
            else if (key == ConsoleKey.Enter)
                break;
        }

        Drink selectedDrink = drinks[drinkIndex];

        dishLogic.LinkDrinkToMainDish(selectedDish.ID, selectedDrink.Id);

        Console.Clear();
        Console.WriteLine("Drink linked successfully.\n");
        Console.WriteLine("| ESC = Go back to menu |\n");
        Console.WriteLine($"Dish : {selectedDish.Name}");
        Console.WriteLine($"Drink: {selectedDrink.Name}");
        Console.ReadKey();
    }
}

    