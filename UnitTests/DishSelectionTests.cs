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
    public sealed class Test_DishSelection
    {
        private readonly DishAccess _dishAccess = new DishAccess();
        private readonly UsersAccess _useraccess = new UsersAccess();
        private readonly ReservationsAccess _reservationaccess = new ReservationsAccess();

        [TestMethod]
        public void ReservedDishes_InsertsIntoDatabase()
        {
            // ARRANGe
            DishModel dish = new DishModel { Name = "dish1", Price = (decimal)10, Description = "Description", Type = "type" };
            _dishAccess.AddDish(dish);
            DishModel dish1 = new DishModel { Name = "dish2", Price = (decimal)10, Description = "Description", Type = "type" };
            _dishAccess.AddDish(dish1);
            DishModel dish2 = new DishModel { Name = "dish3", Price = (decimal)10, Description = "Description", Type = "type" };
            _dishAccess.AddDish(dish2);
            List<DishModel> insertedDishes = _dishAccess.GetDishByType("type");
            List<DishModel> recentDishes = insertedDishes.OrderByDescending(d => d.ID).Take(3).ToList();


            UserModel newuser = new UserModel() { FirstName = "User", LastName = "User", Password = "User", PhoneNumber = "0000000000", EmailAddress = "NewUser@gmail.com", Roles = "customer" };
            _useraccess.DeleteByEmail("NewUser@gmail.com");
            _useraccess.Write(newuser);

            UserModel? newestUser = _useraccess.GetByEmail("NewUser@gmail.com");

            ReservationModel reservering = new ReservationModel()
            {
                UserId = newestUser.ID,
                TableId = 1,
                GuestCount = 1,
                StartAt = DateTime.Today.AddYears(100).ToString(),
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = DateTime.Now.ToString()
            };
            _reservationaccess.Write(reservering);

            ReservationModel insertedReservation = _reservationaccess.GetLatestByUserId(newestUser.ID);


            // act
            DishLogic d = new DishLogic();
            List<int> uitkomsten = d.ReserveDishes(recentDishes, insertedReservation);


            // assert
            Assert.AreEqual(uitkomsten.Count(), 3);
            List<DishModel> dishuitkomsten = _dishAccess.GetAllDishesByReservation(insertedReservation);
            Assert.AreEqual(dishuitkomsten[0].ID, recentDishes[0].ID);
            Assert.AreEqual(dishuitkomsten[1].ID, recentDishes[1].ID);
            Assert.AreEqual(dishuitkomsten[2].ID, recentDishes[2].ID);



            // Clean up
            _dishAccess.DeleteReservationDishes(uitkomsten);
            _dishAccess.Delete(dish);
            _dishAccess.Delete(dish1);
            _dishAccess.Delete(dish2);
            _reservationaccess.Delete(insertedReservation);
            _useraccess.Delete(newestUser);


        }
        
        [TestMethod]
        public void ReservedDishesWithNullDish_InsertsIntoDatabase()
        {
            // ARRANGe
            DishModel dish = new DishModel { Name = "dish1", Price = (decimal)10, Description = "Description", Type = "type" };
            _dishAccess.AddDish(dish);
            DishModel dish1 = new DishModel { Name = "dish2", Price = (decimal)10, Description = "Description", Type = "type" };
            _dishAccess.AddDish(dish1);
            DishModel? dish2 = null;
            List<DishModel> insertedDishes = _dishAccess.GetDishByType("type");
            List<DishModel> recentDishes = insertedDishes.OrderByDescending(d => d.ID).Take(2).ToList();
            recentDishes.Add(dish2);


            UserModel newuser = new UserModel() { FirstName = "User", LastName = "User", Password = "User", PhoneNumber = "0000000000", EmailAddress = "NewUser@gmail.com", Roles = "customer" };
            _useraccess.DeleteByEmail("NewUser@gmail.com");
            _useraccess.Write(newuser);
            
            UserModel? newestUser = _useraccess.GetByEmail("NewUser@gmail.com");

            ReservationModel reservering = new ReservationModel()
            {
                UserId = newestUser.ID,
                TableId = 1,
                GuestCount = 1,
                StartAt = DateTime.Today.AddYears(100).ToString(),
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = DateTime.Now.ToString()
            };
            _reservationaccess.Write(reservering);

            ReservationModel insertedReservation = _reservationaccess.GetLatestByUserId(newestUser.ID);


            // act
            DishLogic d = new DishLogic();
            List<int> uitkomsten = d.ReserveDishes(recentDishes, insertedReservation);


            // assert
            Assert.AreEqual(uitkomsten.Count(), 3);
            List<DishModel> dishuitkomsten = _dishAccess.GetAllDishesByReservation(insertedReservation);
            Assert.AreEqual(dishuitkomsten[0].ID, recentDishes[0].ID);
            Assert.AreEqual(dishuitkomsten[1].ID, recentDishes[1].ID);



            // Clean up
            _dishAccess.DeleteReservationDishes(uitkomsten);
            _dishAccess.Delete(dish);
            _dishAccess.Delete(dish1);
            _reservationaccess.Delete(insertedReservation);
            _useraccess.Delete(newestUser);


        }
    }
    
}

       