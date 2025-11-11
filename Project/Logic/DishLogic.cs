using System.Reflection.Metadata;
using Project.DataModels;
using Project.DataAccess;
using Project.Logic;


namespace Project.Logic
{

    public class DishLogic
    {
        private DishAccess _dishaccess;
        public DishLogic(DishAccess? dishAccess = null)
        {
            _dishaccess = dishAccess ?? new DishAccess();
        }
        private ReservationsLogic _reservationLogic;

        public List<int> ReserveDishes(List<DishModel> reservedDishes, ReservationModel reservation, bool emptyPreviousItems = false)
        {
            if (emptyPreviousItems)
            {
            _dishaccess.DeleteDishesOnReservation(reservation);
            }
            List<int> returnedlist = [];
            foreach (DishModel dish in reservedDishes)
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

        public void DeleteDishesFromReservation(ReservationModel reservation)
        {
            _dishaccess.DeleteDishesOnReservation(reservation);
        }
    }
}