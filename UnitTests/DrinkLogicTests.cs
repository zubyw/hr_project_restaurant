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

    [TestMethod]
    public void SaveAlcoholPercentage()
    {
        drinkLogic.CreateDrink("Bacardi", 6.50m, 40.0);

        List<Drink> drinks = drinkLogic.GetAllDrinks();
        Drink drink = null;

        foreach (Drink d in drinks)
        {
            if (d.Name == "Bacardi")
                drink = d;
        }

        Assert.AreEqual(40.0, drink.AlcoholPercentage);
    }

    [TestMethod]
    public void SavePrice()
    {
        drinkLogic.CreateDrink("Amstel", 4.20m, 5.0);

        List<Drink> drinks = drinkLogic.GetAllDrinks();
        Drink drink = null;

        foreach (Drink d in drinks)
        {
            if (d.Name == "Amstel")
                drink = d;
        }

        Assert.AreEqual(4.20m, drink.Price);
    }

    [TestMethod]
    public void GetById()
    {
        drinkLogic.CreateDrink("Jack Daniels", 7.00m, 40.0);

        List<Drink> drinks = drinkLogic.GetAllDrinks();
        Drink drink = null;

        foreach (Drink d in drinks)
        {
            if (d.Name == "Jack Daniels")
                drink = d;
        }

        Drink result = drinkLogic.GetDrinkById(drink.Id);

        Assert.AreEqual(drink.Id, result.Id);
    }

    [TestMethod]
    public void DeleteDrink()
    {
        drinkLogic.CreateDrink("Grolsch", 4.60m, 5.0);

        List<Drink> drinks = drinkLogic.GetAllDrinks();
        Drink drink = null;

        foreach (Drink d in drinks)
        {
            if (d.Name == "Grolsch")
                drink = d;
        }

        bool deleted = drinkLogic.DeleteDrink(drink.Id);

        List<Drink> afterDelete = drinkLogic.GetAllDrinks();
        bool stillExists = false;

        foreach (Drink d in afterDelete)
        {
            if (d.Name == "Grolsch")
                stillExists = true;
        }

        Assert.IsTrue(deleted);
        Assert.IsFalse(stillExists);
    }
}
