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

    public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Pending")
    {
        if (!CheckGuestCount(guestCount)) return false;
        if (!CheckDateTime(startAt)) return false;

        ReservationModel r = new ReservationModel
        {
            UserId = userId,
            TableId = tableId,
            GuestCount = guestCount,
            StartAt = startAt,
            Status = status,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        _reservationsAccess.Write(r);
        return true;
    }

    // overige CRUD methods ongewijzigd...
}
