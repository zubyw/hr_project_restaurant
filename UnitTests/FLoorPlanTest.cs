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

    public sealed class Test_Floorplan
    {
        private readonly TableAccess _tableAccess = new TableAccess();
        private readonly UsersAccess _useraccess = new UsersAccess();
        private readonly ReservationsAccess _reservationaccess = new ReservationsAccess();

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void ReservedTables_NotGathered(int tableId)
        {
            // Arrange
            UserModel newuser = new UserModel()
            {
                FirstName = "User",
                LastName = "User",
                Password = "User",
                PhoneNumber = "0000000000",
                EmailAddress = "NewUser@gmail.com",
                Roles = "customer"
            };

            _useraccess.DeleteByEmail("NewUser@gmail.com");
            _useraccess.Write(newuser);

            UserModel? newestUser = _useraccess.GetByEmail("NewUser@gmail.com");
            ReservationModel? insertedReservation = null;

            try
            {
                insertedReservation = new ReservationModel()
                {
                    UserId = newestUser.ID,
                    TableId = tableId,
                    GuestCount = 1,
                    StartAt = DateTime.Today.AddYears(100).ToString(),
                    CreatedAt = DateTime.Now.ToString(),
                    UpdatedAt = DateTime.Now.ToString()
                };
                _reservationaccess.Write(insertedReservation);

                insertedReservation = _reservationaccess.GetLatestByUserId(newestUser.ID);

                // Act
                List<int> reservedTableIds = _tableAccess.GetNonAvailableOnDate(
                    insertedReservation.StartAt, insertedReservation.GuestCount);

                // Assert
                Assert.IsTrue(reservedTableIds.Contains(tableId), "Table should be in this list.");
            }
            finally
            {
                if (insertedReservation != null)
                    _reservationaccess.Delete(insertedReservation);

                if (newestUser != null)
                    _useraccess.DeleteByEmail("NewUser@gmail.com");
            }
        }

    }



}
