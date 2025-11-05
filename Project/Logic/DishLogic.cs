using System.Reflection.Metadata;
using Project.DataModels;
using Project.DataAccess;

namespace Project.Logic
{

    public class DishLogic
    {
        private DishAccess _dishaccess;
        public DishLogic(DishAccess? dishAccess = null)
        {
            _dishaccess = dishAccess ?? new DishAccess();
        }

        public List<int> ReserveDishes(List<DishModel> reserveddishes, ReservationModel reservation)
        {
            List<int> returnedlist = [];
            foreach (DishModel dish in reserveddishes)
            {
                if (dish is not null)
                {
                    int x = _dishaccess.ReservedDishes(dish, reservation);
                    returnedlist.Add(x);
                }
            }
            return returnedlist;
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
}