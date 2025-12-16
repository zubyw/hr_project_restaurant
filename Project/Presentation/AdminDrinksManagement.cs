using Project.Logic;
using Project.DataModels;

public static class AdminDrinksManagement
{
    public static void Start()
    {
        string[] options =
        {
            "Create new drink",
            "Manage drinks",
            "Back"
        };

        while (true)
        {
            int index = MenuHelper.ShowMenuUpDown(
                options,
                "=== Admin: Drinks Management ==="
            );

            switch (index)
            {
                case 0:
                    CreateDrink();
                    break;

                case 1:
                    ManageDrinks(); 
                    break;

                case 2:
                    return;
            }
        }
    }
}
