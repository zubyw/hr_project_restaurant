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


        private static readonly string[] _allowedTypes = new string[] 
        { 
         "Starter", 
         "Main", 
         "Dessert" 
        };

        private void EnsureThemeExists(int themeId)
        {
            ThemeAccess access = new ThemeAccess();
            ThemeModel theme = access.GetById(themeId);

            if (theme == null)
            {
                throw new Exception("Theme not found");
            }
        }

        private void EnsureValidType(string type)
        {
            bool valid = false;

            for (int i = 0; i < _allowedTypes.Length; i++)
            {
                if (_allowedTypes[i].Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
            {
                throw new Exception("Type must be Starter, Main or Dessert");
            }
        }

        private void EnsureValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Name is required");
            }
        }

        private void EnsureValidPrice(decimal price)
        {
            if (price <= 0)
            {
            throw new Exception("Price must be greater than 0");
            }
        }


        private void EnsureNotDuplicate(int themeId, string name, string type)
        {
            AdminDishAccess admin = new AdminDishAccess();
            bool exists = admin.ExistsByNameTypeInTheme(themeId, name, type);

            if (exists)
            {
                throw new Exception("Dish already exists for this theme and type");
            }
        }

        public List<DishModel> AdminGetDishesByTheme(int themeId)
        {
            EnsureThemeExists(themeId);

            AdminDishAccess admin = new AdminDishAccess();
            List<DishModel> list = admin.GetByTheme(themeId);

            return list;
        }
    }
}