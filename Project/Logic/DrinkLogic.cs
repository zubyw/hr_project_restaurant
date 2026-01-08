using Project.DataAccess;
using Project.DataModels;
using System.Collections.Generic;

namespace Project.Logic
{
    public class DrinkLogic
    {
        private DrinkAccess _drinkAccess = new DrinkAccess();
        private DishAccess _dishAccess = new DishAccess();

        public void CreateDrink(string name, decimal price, double alcoholPercentage)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than 0.");

            if (alcoholPercentage < 0 || alcoholPercentage > 100)
                throw new ArgumentException("Alcohol percentage must be between 0 and 100.");

            Drink drink = new Drink(0, name, alcoholPercentage, price);
            _drinkAccess.Write(drink);
        }

        public List<Drink> GetAllDrinks()
        {
            return _drinkAccess.GetAll();
        }

        public Drink? GetLinkedDrinkForDish(int dishId)
        {
            DishModel dish = _dishAccess.GetById(dishId);

            if (dish == null || dish.DrinkId == null)
                return null;

            return _drinkAccess.GetById(dish.DrinkId.Value);
        }

        public Drink GetDrinkById(int id)
        {
            return _drinkAccess.GetById(id);
        }

        public void UpdateDrink(Drink drink)
        {
            if (drink == null)
                throw new ArgumentNullException(nameof(drink));

            if (string.IsNullOrWhiteSpace(drink.Name))
                throw new ArgumentException("Name cannot be empty.");

            if (drink.Price < 0)
                throw new ArgumentException("Price cannot be negative.");

            if (drink.Price == 0)
                throw new ArgumentException("Price must be greater than 0.");

            if (drink.AlcoholPercentage < 0 || drink.AlcoholPercentage > 100)
                throw new ArgumentException("Alcohol percentage must be between 0 and 100.");
            _drinkAccess.Update(drink);
        }

        public bool DeleteDrink(Drink drink)
        {
            if (_drinkAccess.IsDrinkLinked(drink.ID))
            {
                return false;
            }

            _drinkAccess.Delete(drink);
            return true;
        }

        public Drink GetDrinkForDish(int dishId)
        {
            DishModel dish = _dishAccess.GetById(dishId);

            if (dish == null || dish.DrinkId == null)
                return null;

            return _drinkAccess.GetById(dish.DrinkId.Value);
        }
    }
}
