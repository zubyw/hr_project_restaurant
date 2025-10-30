using System.Reflection.Metadata;
using Project.DataModels;

public class DishLogic
{
    private DishAccess _dishaccess = new();


    public void ReserveDishes(List<DishModel> reserveddishes, ReservationModel reservation)
    {
        foreach (DishModel dish in reserveddishes)
        {
            _dishaccess.ReservedDishes(dish, reservation);
        }
    }

    
}