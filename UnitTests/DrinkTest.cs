using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.DataAccess;
using Project.DataModels;
using Project.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class Test_AdminManageDrinks
    {
        private DrinkAccess _drinkAccess; // Renamed to avoid ambiguity
        private DrinkLogic _adminDrinkLogic; // Renamed to avoid ambiguity

        [TestInitialize]
        public void Setup()
        {
            _drinkAccess = new DrinkAccess(); // Initialize renamed field
            _adminDrinkLogic = new DrinkLogic(); // Initialize renamed field
        }

        [TestMethod]
        public void AdminCanCreateAndRetrieveDrink()
        {
            // ARRANGE
            string name = "TestCola";
            decimal price = 2.50m;
            double alcohol = 0;

            // ACT
            _adminDrinkLogic.CreateDrink(name, price, alcohol);
            List<Drink> drinks = _drinkAccess.GetAll();
            Drink createdDrink = drinks.Last();

            // ASSERT
            Assert.AreEqual(name, createdDrink.Name);
            Assert.AreEqual(price, createdDrink.Price);
            Assert.AreEqual(alcohol, createdDrink.AlcoholPercentage);

            _drinkAccess.Delete(createdDrink);
        }

        [TestMethod]
        public void AdminCanUpdateDrink()
        {
            // ARRANGE
            Drink drink = new Drink(0, "TestBeer", 5, 3.00m);
            _drinkAccess.Write(drink);
            Drink insertedDrink = _drinkAccess.GetAll().Last();
            insertedDrink.Name = "UpdatedBeer";
            insertedDrink.Price = 3.50m;

            // ACT
            _adminDrinkLogic.UpdateDrink(insertedDrink);
            Drink updatedDrink = _drinkAccess.GetById(insertedDrink.ID);

            // ASSERT
            Assert.AreEqual("UpdatedBeer", updatedDrink.Name);
            Assert.AreEqual(3.50m, updatedDrink.Price);

            _drinkAccess.Delete(updatedDrink);
        }

        [TestMethod]
        public void DrinkWithInvalidDataIsNotCreated()
        {
            // ARRANGE
            string name = "";
            decimal price = -1;
            double alcohol = 150;

            // ACT & ASSERT
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _adminDrinkLogic.CreateDrink(name, price, alcohol);
            });
        }

        [TestMethod]
        public void DrinkNotLinkedToDishCanBeDeleted()
        {
            // ARRANGE
            Drink drink = new Drink(0, "UnlinkedDrink", 0, 2.00m);
            _drinkAccess.Write(drink);
            Drink insertedDrink = _drinkAccess.GetAll().Last();
            bool isLinked = _drinkAccess.IsDrinkLinked(insertedDrink.ID);
            Assert.IsFalse(isLinked);

            // ACT
            bool result = _adminDrinkLogic.DeleteDrink(insertedDrink);

            // ASSERT
            Assert.IsTrue(result);

            _drinkAccess.Delete(insertedDrink);
        }
    }

    [TestClass]
    public class DrinkLogicTests
    {
        private DrinkLogic _logic; // Renamed to avoid ambiguity

        [TestInitialize]
        public void Setup()
        {
            _logic = new DrinkLogic(); // Initialize renamed field
        }

        [TestMethod]
        public void CreateDrink_ValidInput_ShouldSucceed()
        {
            // ARRANGE
            string name = "ValidDrink";
            decimal price = 5.00m;
            double alcoholPercentage = 10.0;

            // ACT
            _logic.CreateDrink(name, price, alcoholPercentage); // Updated field name

            // ASSERT
            List<Drink> drinks = _logic.GetAllDrinks(); // Updated field name
            Drink createdDrink = drinks.Last();
            Assert.AreEqual(name, createdDrink.Name);
            Assert.AreEqual(price, createdDrink.Price);
            Assert.AreEqual(alcoholPercentage, createdDrink.AlcoholPercentage);
        }

        [TestMethod]
        public void GetAllDrinks_ShouldReturnList()
        {
            // Act
            List<Drink> drinks = _logic.GetAllDrinks();

            // Assert
            Assert.IsNotNull(drinks);
        }
    }
}
