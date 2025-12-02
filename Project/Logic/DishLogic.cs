using System.Reflection.Metadata;
using Project.DataModels;
using Project.DataAccess;
using Project.Logic;
using System.Dynamic;


namespace Project.Logic
{

    public class DishLogic
    {
        private DishAccess _dishaccess;

        private ReservationsAccess _reservationsAccess = new ReservationsAccess();


        public DishLogic(DishAccess? dishAccess = null)
        {
            _dishaccess = dishAccess ?? new DishAccess();
        }

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
        public void WriteIntoDB(DishModel dish)
        {
            _dishaccess.Write(dish);
        }

        public int WriteIntoDBAndReturnId(DishModel dish)
        {
            return _dishaccess.AddDishReturnId(dish);
        }

        public ThemeModel? GetCorrectTheme(string date)
        {
            DateTime parsedDate = DateTime.Parse(date);
            string formattedDate = parsedDate.ToString("yyyy-MM");
            formattedDate += "-01";
            var themeAccess = new ThemeAccess();
            return themeAccess.GetCorrectTheme(formattedDate);
        }

        public void DeleteDishesFromReservation(ReservationModel reservation)
        {
            _dishaccess.DeleteDishesOnReservation(reservation);
        }

        // Admin methods theme / dishes management
        private static readonly string[] _allowedTypes = new string[]
        {
            "Starter",
            "Main",
            "Dessert"
        };
        public bool DoesDishExist(string dishname)
        {
            return _dishaccess.GetDishByName(dishname);
        }

        public List<DishModel> GetAllDishes()
        {
            return _dishaccess.GetAllDishes();
        }
        public void UpdateDish(DishModel dish)
        {
            _dishaccess.Update(dish);
        }

        public void DeleteDishInDishes(DishModel dish)
        {
            _dishaccess.Delete(dish);
        }

        public void DeleteDishInDishes_Themes(DishModel dish)
        {
            _dishaccess.DeleteDishes_Themes(dish);
        }

        public void DeleteDishInReservations_Dishes(DishModel dish)
        {
            _dishaccess.DeleteReservationDishes(dish);
        }
    
    
        public List<(string DishName, int Count)> GetDishCountsForDate(string date)
        {
            return _reservationsAccess.GetDishCountsByDate(date);
        }

    }
}