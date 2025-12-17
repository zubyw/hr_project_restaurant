using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.Logic;
using Project.DataModels;
using System.Collections.Generic;

[TestClass]
public class DrinkLogicTests
{
    private DrinkLogic drinkLogic;

    [TestInitialize]
    public void Setup()
    {
        drinkLogic = new DrinkLogic();
    }

    [TestMethod]
    public void AddDrink()
    {
        drinkLogic.CreateDrink("Heineken", 4.50m, 5.0);

        List<Drink> drinks = drinkLogic.GetAllDrinks();

        bool found = false;
        foreach (Drink d in drinks)
        {
            if (d.Name == "Heineken")
                found = true;
        }

        Assert.IsTrue(found);
    }
}
