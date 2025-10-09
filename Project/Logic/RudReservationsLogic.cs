using System.Collections.Generic;
using Project.DataAccess;
using Project.DataModels;

namespace Project.Logic
{
    // logic class that handles the reservation
    public class RudReservationsLogic
    {
        // instance DataAccess
        RudReservationsAccess access = new RudReservationsAccess();

        // method that gets all reservations with userId
        public List<ReservationModel> GetReservations(int userId)
        {
            return access.GetReservationsByUserId(userId);
        }

        // method that updates reservation
        public void UpdateReservation(int id, int guestCount, string startAt)
        {
            if (guestCount <= 0 || string.IsNullOrEmpty(startAt))
            return;

            access.UpdateReservation(id, guestCount, startAt);
        }

        // method that deletes reservation
        public void DeleteReservation(int id)
        {
            access.DeleteReservation(id);
        }
    }
}


