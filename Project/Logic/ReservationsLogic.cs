using Project.DataModels;
using System;
using System.Linq;

public class ReservationsLogic
{
    private ReservationsAccess _reservationsAccess = new ReservationsAccess();

    public List<ReservationModel> GetAllReservations()
    {
        return _reservationsAccess.GetAll();
    }

    public List<ReservationModel> GetReservationsByDate(string date)
    {
        return _reservationsAccess.GetByDate(date);
    }

    public List<ReservationModel> GetReservationsByDateRange(string startDate, string endDate)
    {
        return _reservationsAccess.GetByDateRange(startDate, endDate);
    }

    public List<ReservationModel> GetReservationsByUserId(int userId)
    {
        return _reservationsAccess.GetByUserId(userId);
    }

    public ReservationModel? GetReservationById(int id)
    {
        return _reservationsAccess.GetById(id);
    }

    public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Pending")
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

    public bool UpdateReservationStatus(int reservationId, string newStatus)
    {
        var reservation = _reservationsAccess.GetById(reservationId);
        if (reservation == null) return false;

        reservation.Status = newStatus;
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _reservationsAccess.Update(reservation);
        return true;
    }

    public bool DeleteReservation(int reservationId)
    {
        _reservationsAccess.DeleteById(reservationId);
        return true;
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

    // vanaf hier heb ik het verder aangevuld. Ook hier heb ik de datamodels gebruikt ten opzichte van vorige keer:

public bool ChangeReservationTime(int reservationId, DateTime newTime)
{
    if (newTime <= DateTime.Now) return false;

    ReservationModel reservation = _reservationsAccess.GetById(reservationId);
    if (reservation == null) return false;

    reservation.StartAt = newTime.ToString("yyyy-MM-dd HH:mm:ss");
    reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    _reservationsAccess.Update(reservation);
    return true;
}

public bool ChangeReservationPersons(int reservationId, int newGuestCount)
{
    if (newGuestCount != 2 && newGuestCount != 4 && newGuestCount != 6) return false;

    ReservationModel reservation = _reservationsAccess.GetById(reservationId);
    if (reservation == null) return false;

    if (reservation.TableCapacity >= newGuestCount)
    {
        reservation.GuestCount = newGuestCount;
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _reservationsAccess.UpdateGuestCount(reservation);
        return true;
    }

    // Zoek een geschikte vrije tafel
    System.Collections.Generic.List<TableModel> availableTables = _reservationsAccess.GetFreeTables(reservation);
    if (availableTables.Count == 0) return false;

    // Werk het reservation  bij met de nieuwe tafel en (nieuwe) guest count
    TableModel newTable = availableTables[0];
    reservation.TableId = newTable.ID;
    reservation.GuestCount = newGuestCount;
    reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    _reservationsAccess.UpdateReservationTable(reservation);
    return true;
}

public bool CancelReservation(int reservationId)
{
    ReservationModel reservation = _reservationsAccess.GetById(reservationId);
    if (reservation == null) return false;

    reservation.Status = "Geannuleerd";
    reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    _reservationsAccess.CancelReservation(reservation);
    return true;
}


    // Methods for guest reservation management (merged from RudReservationsLogic)
    public List<ReservationModel> GetReservationsByUserIdForGuest(int userId)
    {
        return _reservationsAccess.GetReservationsByUserIdSimple(userId);
    }

    public void UpdateReservationForGuest(int id, int guestCount, string startAt)
    {
        if (guestCount <= 0 || string.IsNullOrEmpty(startAt))
            return;

        _reservationsAccess.UpdateReservationSimple(id, guestCount, startAt);
    }

    public void DeleteReservationForGuest(int id)
    {
        _reservationsAccess.DeleteReservationSimple(id);
    }

    public bool IsValidReservationDateTime(string input)
    {
        DateTime date;

        // check if date/time format is correct
        if (!DateTime.TryParse(input, out date))
            return false;

        int hour = date.Hour;
        if (hour >= 17)
            return true;

        return false;
    }

}
