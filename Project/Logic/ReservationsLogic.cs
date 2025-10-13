using Project.DataModels;

public class ReservationsLogic
{
    private ReservationsAccess _reservationsAccess = new ReservationsAccess();

    public List<ReservationModel> GetAllReservations()
    {
        try
        {
            return _reservationsAccess.GetAll();
        }
        catch
        {
            return new List<ReservationModel>();
        }
    }

    public List<ReservationModel> GetReservationsByDate(string date)
    {
        try
        {
            return _reservationsAccess.GetByDate(date);
        }
        catch
        {
            return new List<ReservationModel>();
        }
    }

    public List<ReservationModel> GetReservationsByDateRange(string startDate, string endDate)
    {
        try
        {
            return _reservationsAccess.GetByDateRange(startDate, endDate);
        }
        catch
        {
            return new List<ReservationModel>();
        }
    }

    public List<ReservationModel> GetReservationsByUserId(int userId)
    {
        try
        {
            return _reservationsAccess.GetByUserId(userId);
        }
        catch
        {
            return new List<ReservationModel>();
        }
    }

    public ReservationModel? GetReservationById(int id)
    {
        try
        {
            return _reservationsAccess.GetById(id);
        }
        catch
        {
            return null;
        }
    }

    public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Pending")
    {
        try
        {
            var reservation = new ReservationModel
            {
                UserId = userId,
                TableId = tableId,
                GuestCount = guestCount,
                StartAt = startAt,
                Status = status,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _reservationsAccess.Write(reservation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool UpdateReservationStatus(int reservationId, string newStatus)
    {
        try
        {
            var reservation = _reservationsAccess.GetById(reservationId);
            if (reservation == null) return false;

            reservation.Status = newStatus;
            reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _reservationsAccess.Update(reservation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteReservation(int reservationId)
    {
        try
        {
            _reservationsAccess.DeleteById(reservationId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Helper methods for date formatting and validation
    public static string FormatDateForDatabase(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    public static string FormatDateTimeForDatabase(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static bool IsValidDateFormat(string dateString)
    {
        return DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
    }

    // vanaf hier heb ik het verder aangevuld.

        public bool ChangeReservationTime(int reservationId, DateTime newTime)
    {
        try
        {
            if (newTime <= DateTime.Now)
            {
                return false;
            }

            ReservationModel reservation = _reservationsAccess.GetById(reservationId);
            if (reservation == null)
            {
                return false;
            }

            reservation.StartAt = newTime.ToString("yyyy-MM-dd HH:mm:ss");
            reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _reservationsAccess.Update(reservation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ChangeReservationPersons(int reservationId, int newGuestCount)
    {
        try
        {
            if (newGuestCount != 2 && newGuestCount != 4 && newGuestCount != 6)
            {
                return false;
            }

            ReservationModel reservation = _reservationsAccess.GetById(reservationId);
            if (reservation == null)
            {
                return false;
            }

            reservation.GuestCount = newGuestCount;
            reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _reservationsAccess.Update(reservation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool CancelReservation(int reservationId)
    {
        try
        {
            ReservationModel reservation = _reservationsAccess.GetById(reservationId);
            if (reservation == null)
            {
                return false;
            }

            reservation.Status = "Geannuleerd";
            reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _reservationsAccess.Update(reservation);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
