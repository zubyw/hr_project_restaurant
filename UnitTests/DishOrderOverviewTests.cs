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
    public sealed class Test_AdminDishOrdersPerDay
    {
        private readonly DishAccess _dishAccess = new DishAccess();
        private readonly UsersAccess _userAccess = new UsersAccess();
        private readonly ReservationsAccess _reservationAccess = new ReservationsAccess();

        [TestMethod]
        public void AdminCanSeeOrderedDishesForGivenDate()
        {
            // ARRANGE: Maak gerechten aan
            DishModel dish1 = new DishModel { Name = "Pizza", Price = 10, Description = "Cheese", Type = "Main" };
            DishModel dish2 = new DishModel { Name = "Pasta", Price = 12, Description = "Tomato", Type = "Main" };
            _dishAccess.Write(dish1);
            _dishAccess.Write(dish2);

            List<DishModel> insertedDishes = _dishAccess.GetDishByType("Main")
            .OrderByDescending(d => d.ID)
            .Take(2).ToList();

            // ARRANGE: Maak gebruiker aan
            UserModel user = new UserModel
            {
                FirstName = "AdminTest",
                LastName = "User",
                Password = "1234",
                PhoneNumber = "0000000000",
                EmailAddress = "adminTestUser@gmail.com",
                Roles = "customer"
            };
            _userAccess.DeleteByEmail(user.EmailAddress); // cleanup als hij al bestaat
            _userAccess.Write(user);
            UserModel newestUser = _userAccess.GetByEmail(user.EmailAddress);

            // ARRANGE: Maak reservering aan
            string testDate = DateTime.Today.ToString("yyyy-MM-dd");
            ReservationModel reservation = new ReservationModel
            {
                UserId = newestUser.ID,
                TableId = 1,
                GuestCount = 2,
                StartAt = testDate,
                Status = "confirmed",
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = DateTime.Now.ToString()
            };
            _reservationAccess.Write(reservation);
            ReservationModel insertedReservation = _reservationAccess.GetLatestByUserId(newestUser.ID);

            // ACT: Reserveer gerechten
            DishLogic dishLogic = new DishLogic();
            List<int> reservedIds = dishLogic.ReserveDishes(insertedDishes, insertedReservation);

            // ACT: Haal bestellingen op voor de datum
            List<(string DishName, int Count)> dishCounts = dishLogic.GetDishCountsForDate(testDate);

            // ASSERT: Controleer dat alle gerechten worden weergegeven met juiste aantallen
            Assert.IsTrue(dishCounts.Any(x => x.DishName == "Pizza" && x.Count == 1));
            Assert.IsTrue(dishCounts.Any(x => x.DishName == "Pasta" && x.Count == 1));

            // CLEAN UP
            _dishAccess.DeleteReservationDishes(reservedIds);
            _dishAccess.Delete(dish1);
            _dishAccess.Delete(dish2);
            _reservationAccess.Delete(insertedReservation);
            _userAccess.Delete(newestUser);
        }

        [TestMethod]
        public void AdminSeesNoOrdersForDateWithNoReservations()
        {
            // ARRANGE: Kies een datum waarvan we weten dat er geen orders zijn
            string emptyDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            DishLogic dishLogic = new DishLogic();

            // ACT
            List<(string DishName, int Count)> dishCounts = dishLogic.GetDishCountsForDate(emptyDate);

            // ASSERT
            Assert.AreEqual(0, dishCounts.Count);
        }

        [TestMethod]
        public void InvalidDateFormatThrowsException()
        {
            // ARRANGE
            string invalidDate = "invalid-date";

            // ACT & ASSERT
            Assert.ThrowsException<FormatException>(() => DateTime.Parse(invalidDate));
        }
    }
}
