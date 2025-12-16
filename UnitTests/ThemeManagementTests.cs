using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.DataAccess;
using Project.DataModels;
using System.Linq;
using Project.Logic;

namespace UnitTests
{
    [TestClass]
    public sealed class Test_ThemeManagement
    {
        private readonly DishAccess _dishAccess = new DishAccess();

        private readonly DishLogic _dishlogic = new DishLogic();
        private static readonly ThemesLogic _themelogic = new ThemesLogic();

        private static readonly ThemeAccess _themeAccess = new ThemeAccess();


        [TestMethod]
        public void availableDishesCheck()
        {
            //arrange
            DishModel dish = new DishModel { Name = "dish1", Price = (decimal)10, Description = "Description", Type = "ThemeCheck" };
            DishModel dish2 = new DishModel { Name = "dish2", Price = (decimal)10, Description = "Description", Type = "ThemeCheck2" };
            DishModel dish3 = new DishModel { Name = "dish3", Price = (decimal)10, Description = "Description", Type = "ThemeCheck3" };
            _dishAccess.Write(dish);
            _dishAccess.Write(dish2);
            _dishAccess.Write(dish3);

            ThemeModel theme1 = new ThemeModel {Name = "Theme1", Course = "Theme1"};
            ThemeModel theme2 = new ThemeModel {Name = "Theme2", Course = "Theme2"};
            ThemeModel theme3 = new ThemeModel {Name = "Theme3", Course = "Theme3"};
            _themelogic.WriteTheme(theme1);
            _themelogic.WriteTheme(theme2);
            _themelogic.WriteTheme(theme3);


            // Also get both the Id from the data base since it is autoincrimated
            List<DishModel> dishinlist = _dishAccess.GetDishByType("ThemeCheck");
            List<DishModel> dishinlist2 = _dishAccess.GetDishByType("ThemeCheck2");
            List<DishModel> dishinlist3 = _dishAccess.GetDishByType("ThemeCheck3");
            DishModel reloadedDish = dishinlist[0];
            DishModel reloadedDish2 = dishinlist2[0];
            DishModel reloadedDish3 = dishinlist3[0];
            ThemeModel? reloadedTheme = _themeAccess.GetByName("Theme1");
            ThemeModel? reloadedTheme2 = _themeAccess.GetByName("Theme2");
            ThemeModel? reloadedTheme3 = _themeAccess.GetByName("Theme3");

            _themelogic.AddDishesToTheme(dishinlist, reloadedTheme);
            _themelogic.AddDishesToTheme(dishinlist2, reloadedTheme2);
            _themelogic.AddDishesToTheme(dishinlist3, reloadedTheme3);


            // act
            List<DishModel> dishes = _themelogic.GetAllAvailableDishes(reloadedTheme);
            List<DishModel> dishes2 = _themelogic.GetAllAvailableDishes(reloadedTheme2);
            List<DishModel> dishes3 = _themelogic.GetAllAvailableDishes(reloadedTheme3);
            List<DishModel> dishInTheme = _dishAccess.GetByTheme(reloadedTheme);
            List<DishModel> dishInTheme2 = _dishAccess.GetByTheme(reloadedTheme2);
            List<DishModel> dishInTheme3 = _dishAccess.GetByTheme(reloadedTheme3);

            // assert

            Assert.IsFalse(dishes.Any(d => d.ID == reloadedDish.ID), "Dish should not be in this list");
            Assert.IsFalse(dishes2.Any(d => d.ID == reloadedDish2.ID), "Dish should not be in this list");
            Assert.IsFalse(dishes3.Any(d => d.ID == reloadedDish3.ID), "Dish should not be in this list");

            Assert.IsTrue(dishes.Any(d => d.ID == reloadedDish2.ID), "Dish that isn't linked should be in this list.");
            Assert.IsTrue(dishes2.Any(d => d.ID == reloadedDish3.ID), "Dish that isn't linked should be in this list.");
            Assert.IsTrue(dishes3.Any(d => d.ID == reloadedDish.ID), "Dish that isn't linked should be in this list.");

            Assert.IsTrue(dishInTheme.Any(d => d.ID == reloadedDish.ID), "Dish should be in this list.");
            Assert.IsTrue(dishInTheme2.Any(d => d.ID == reloadedDish2.ID), "Dish should be in this list.");
            Assert.IsTrue(dishInTheme3.Any(d => d.ID == reloadedDish3.ID), "Dish should be in this list.");

            // delete

            _dishlogic.DeleteDishInDishes_Themes(reloadedDish);
            _dishlogic.DeleteDishInReservations_Dishes(reloadedDish);
            _dishlogic.DeleteDishInDishes(reloadedDish);
            _dishlogic.DeleteDishInDishes_Themes(reloadedDish2);
            _dishlogic.DeleteDishInReservations_Dishes(reloadedDish2);
            _dishlogic.DeleteDishInDishes(reloadedDish2);
            _dishlogic.DeleteDishInDishes_Themes(reloadedDish3);
            _dishlogic.DeleteDishInReservations_Dishes(reloadedDish3);
            _dishlogic.DeleteDishInDishes(reloadedDish3);

            _themelogic.DeleteThemeCompletely(reloadedTheme);
            _themelogic.DeleteThemeCompletely(reloadedTheme2);
            _themelogic.DeleteThemeCompletely(reloadedTheme3);



        }
        [TestMethod]
        [DataRow("Turks", "Turkse maaltijden", "02-2026", "03-2026")]
        [DataRow("Colombiaans", "Colombiaanse maaltijden", "04-2026", "05-2026")]
        [DataRow("Grieks", "Griekse maaltijden", "06-2026", "07-2026")]
        [DataRow("Fries", "Oud Hollandse Frieze gerechten", "08-2026", "09-2026")]
        
        public void availableMonthsCheck(string tName, string tDescription, string datum1, string datum2)
        {
            //arrange
            ThemeModel theme = new ThemeModel {Name = tName, Course = tDescription};
            _themelogic.WriteTheme(theme);
            ThemeModel? reloadedTheme = _themeAccess.GetByName(tName);
            List<string> datums = [];
            datums.Add(DateTime.ParseExact(datum1, "MM-yyyy", null).ToString("yyyy-MM"));
            datums.Add(DateTime.ParseExact(datum2, "MM-yyyy", null).ToString("yyyy-MM"));


            // act
            _themelogic.LinkMonthsToTheme(datums, reloadedTheme);
            List<string> availableMonths = _themelogic.GetAvailableMonths();
            List<string> timeslots = _themeAccess.GetThemeCalendarTakenMonths();
        
            List<string> takenMonths = timeslots
                .Select(ts => DateTime.Parse(ts))
                .Where(d => d >= DateTime.Today)
                .OrderBy(d => d)
                .Select(d => d.ToString("MM-yyyy"))
                .ToList();
            
            // assert
            // Make sure the dates are not in the availablemonths list
            Assert.IsFalse(availableMonths.Contains(datum1), $"{datum1} Should not be in this list");
            Assert.IsFalse(availableMonths.Contains(datum2), $"{datum2} Should not be in this list");

            //make sure that the months are actually linked to the theme
            Assert.IsTrue(takenMonths.Contains(datum1), $"{datum1} Should be in this list");
            Assert.IsTrue(takenMonths.Contains(datum2), $"{datum2} Should be in this list");

            // delete
            _themeAccess.DeleteThemeCompletely(reloadedTheme);       
        }
    }
}