using Project.DataAccess;
using Project.DataModels;
using System.Collections.Generic;

namespace Project.Logic
{
    public class DrinkLogic
    {
        private DrinkAccess _drinkAccess = new DrinkAccess();

        public void CreateDrink(string name, double alcoholPercentage, decimal price)
        {
            Drink drink = new Drink(0, name, alcoholPercentage, price);
            _drinkAccess.Write(drink);
        }

        public List<Drink> GetAllDrinks()
        {
            return _drinkAccess.GetAll();
        }
    }
}
