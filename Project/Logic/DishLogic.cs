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

    public List<DishModel> GetDishesByType(string type)
    {
        return _dishaccess.GetDishByType(type);
    }

    public List<DishModel> GetDishesByTheme(int themeId)
    {
        var dishIds = _dishaccess.GetallDishIdByThemeId(themeId);
        if (dishIds.Count > 0)
        {
            return _dishaccess.GetDishesByIds(dishIds);
        }
        return new List<DishModel>();
    }

    public int? GetCurrentThemeId()
    {
        var themeAccess = new ThemeAccess();
        return themeAccess.GetActiveThemeID();
    }
}