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
    public sealed class ReservationAndMenuTests
    {
        private readonly DishAccess _dishAccess = new DishAccess();
        private readonly UsersAccess _userAccess = new UsersAccess();
        private readonly ReservationsAccess _reservationAccess = new ReservationsAccess();
        private readonly DishLogic _dishLogic = new DishLogic();

        [TestMethod]
        public void AdminCanAddAndEditThemesAndDishes()
        {
            ThemesLogic themesLogic = new ThemesLogic();

            // Maak nieuw thema
            ThemeModel theme = new ThemeModel
            {
                Name = "Test Theme",
                Course = "Monthly Specials"
            };
            themesLogic.WriteTheme(theme);

            ThemeModel? insertedTheme = themesLogic.GetAll().FirstOrDefault(t => t.Name == "Test Theme");
            Assert.IsNotNull(insertedTheme);

            // Maak nieuw gerecht
            DishModel dish = new DishModel
            {
                Name = "Test Dish",
                Price = 10,
                Description = "Test Description",
                Type = "Main"
            };
            _dishAccess.Write(dish);
            DishModel insertedDish = _dishAccess.GetAllDishes().FirstOrDefault(d => d.Name == "Test Dish")!;
            Assert.IsNotNull(insertedDish);

            // Link gerecht aan thema
            themesLogic.AddDishesToTheme(new List<DishModel> { insertedDish }, insertedTheme!);
            List<DishModel> dishesInTheme = themesLogic.GetAllDishesInTheme(insertedTheme!);
            Assert.IsTrue(dishesInTheme.Any(d => d.ID == insertedDish.ID));

            // Cleanup
            themesLogic.DeleteDishonTheme(insertedDish, insertedTheme!);
            _dishAccess.Delete(insertedDish);
        }

        [TestMethod]
        public void GuestCanMakeAndCancelReservation()
        {
            // Maak gast
            UserModel guest = new UserModel
            {
                FirstName = "GuestTest",
                LastName = "User",
                Password = "1234",
                PhoneNumber = "0000000000",
                EmailAddress = "guestTest@gmail.com",
                Roles = "customer"
            };
            _userAccess.DeleteByEmail(guest.EmailAddress);
            _userAccess.Write(guest);
            guest = _userAccess.GetByEmail(guest.EmailAddress)!;

            // Maak tafelreservering
            string testDate = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy HH:mm");
            ReservationModel reservation = new ReservationModel
            {
                UserId = guest.ID,
                TableId = 1,
                GuestCount = 2,
                StartAt = testDate,
                Status = "Open",
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = DateTime.Now.ToString()
            };
            _reservationAccess.Write(reservation);
            ReservationModel insertedReservation = _reservationAccess.GetLatestByUserId(guest.ID);

            Assert.IsNotNull(insertedReservation);
            Assert.AreEqual(guest.ID, insertedReservation.UserId);

            // Annuleer reservering
            ReservationsLogic reservationsLogic = new ReservationsLogic();
            ReservationsLogic.CurrentUserId = guest.ID;
            bool canceled = reservationsLogic.CancelReservation(insertedReservation.ID);
            Assert.IsTrue(canceled);

            // Cleanup
            _reservationAccess.Delete(insertedReservation);
            _userAccess.Delete(guest);
        }

        [TestMethod]
        public void AdminCanSeeDishOrdersForDate()
        {
            // Maak gerecht
            DishModel dish = new DishModel
            {
                Name = "Pizza",
                Price = 10,
                Description = "Cheese",
                Type = "Main"
            };
            _dishAccess.Write(dish);
            DishModel insertedDish = _dishAccess.GetAllDishes().First(d => d.Name == "Pizza");

            // Maak gast
            UserModel guest = new UserModel
            {
                FirstName = "AdminTest",
                LastName = "User",
                Password = "1234",
                PhoneNumber = "0000000000",
                EmailAddress = "adminTestUser@gmail.com",
                Roles = "customer"
            };
            _userAccess.DeleteByEmail(guest.EmailAddress);
            _userAccess.Write(guest);
            guest = _userAccess.GetByEmail(guest.EmailAddress)!;

            // Maak reservering
            string testDate = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy HH:mm");
            ReservationModel reservation = new ReservationModel
            {
                UserId = guest.ID,
                TableId = 1,
                GuestCount = 2,
                StartAt = testDate,
                Status = "confirmed",
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = DateTime.Now.ToString()
            };
            _reservationAccess.Write(reservation);
            ReservationModel insertedReservation = _reservationAccess.GetLatestByUserId(guest.ID);

            // Reserveer gerecht
            List<int> reservedDishIds = _dishLogic.ReserveDishes(new List<DishModel> { insertedDish }, insertedReservation);

            // Haal bestellingen op voor datum
            List<(string DishName, int Count)> dishCounts = _dishLogic.GetDishCountsForDate(testDate);
            Assert.IsTrue(dishCounts.Any(x => x.DishName == "Pizza" && x.Count >= 1));

            // Cleanup
            _dishAccess.DeleteReservationDishes(reservedDishIds);
            _reservationAccess.Delete(insertedReservation);
            _dishAccess.Delete(insertedDish);
            _userAccess.Delete(guest);
        }

        [TestMethod]
        public void GuestCanLogin()
        {
            // Maak gast
            UserModel guest = new UserModel
            {
                FirstName = "LoginTest",
                LastName = "User",
                Password = "1234",
                PhoneNumber = "0000000000",
                EmailAddress = "loginTestUser@gmail.com",
                Roles = "customer"
            };
            _userAccess.DeleteByEmail(guest.EmailAddress);
            _userAccess.Write(guest);

            UsersLogic usersLogic = new UsersLogic();
            UserModel? loggedInUser = usersLogic.CheckLogin("loginTestUser@gmail.com", "1234");
            Assert.IsNotNull(loggedInUser);
            Assert.AreEqual(guest.EmailAddress, loggedInUser!.EmailAddress);

            _userAccess.Delete(loggedInUser);
        }
    }
}

