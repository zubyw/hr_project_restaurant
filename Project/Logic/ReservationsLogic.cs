using System;
using System.Collections.Generic;
using System.Globalization;
using Project.DataModels;

public class ReservationsLogic
{
    private ReservationsAccess _reservationsAccess = new ReservationsAccess();

    private static readonly List<string> ValidTimes = new List<string>
    {
        "17:00","17:30","18:00","18:30","19:00","19:30","20:00","20:30"
    };

    private bool CheckGuestCount(int guestCount)
    {
        return guestCount >= 1 && guestCount <= 6;
    }

    private bool CheckTime(string time)
    {
        return ValidTimes.Contains(time);
    }

    private bool CheckDateTime(string startAt)
    {
        DateTime date;
        if (!DateTime.TryParseExact(startAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            return false;
        if (date <= DateTime.Now) return false;
        string time = date.ToString("HH:mm");
        return CheckTime(time);
    }

    private bool IsOwner(int reservationId, int userId)
    {
        ReservationModel res = _reservationsAccess.GetById(reservationId);
        if (res == null) return false;
        return res.UserId == userId;
    }

    public bool UpdateReservationForGuest(int id, int userId, int guestCount, string startAt)
    {
        if (!IsOwner(id, userId)) return false;
        if (!CheckGuestCount(guestCount)) return false;
        if (!CheckDateTime(startAt)) return false;

        _reservationsAccess.UpdateReservationSimple(id, guestCount, startAt);
        return true;
    }

    public bool DeleteReservationForGuest(int id, int userId)
    {
        if (!IsOwner(id, userId)) return false;
        _reservationsAccess.DeleteReservationSimple(id);
        return true;
    }
}
