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
            DishAccess admin = new DishAccess();
            bool exists = admin.ExistsByNameTypeInTheme(themeId, name, type);

            if (exists)
            {
                throw new Exception("Dish already exists for this theme and type");
            }
        }

        public List<DishModel> AdminGetDishesByTheme(int themeId)
        {
            EnsureThemeExists(themeId);

            DishAccess admin = new DishAccess();
            List<DishModel> list = admin.GetByTheme(themeId);

            return list;
        }

        public int AdminAddDishToTheme(int themeId, string name, decimal price, string description, string type)
        {
            EnsureThemeExists(themeId);
            EnsureValidName(name);
            EnsureValidPrice(price);
            EnsureValidType(type);
            EnsureNotDuplicate(themeId, name, type);

            DishModel dish = new DishModel();
            dish.Name = name;
            dish.Price = price;
            dish.Description = description;
            dish.Type = type;

            int newId = _dishaccess.AddDishReturnId(dish);
            _dishaccess.LinkDishToTheme(newId, themeId);

            return newId;
        }

        public void AdminUpdateDishInTheme(int dishId, int themeId, string name, decimal price, string description, string type)
        {
            EnsureThemeExists(themeId);
            EnsureValidName(name);
            EnsureValidPrice(price);
            EnsureValidType(type);

            DishModel dish = new DishModel();
            dish.ID = dishId;
            dish.Name = name;
            dish.Price = price;
            dish.Description = description;
            dish.Type = type;

            _dishaccess.Update(dish);
        }

    }
}