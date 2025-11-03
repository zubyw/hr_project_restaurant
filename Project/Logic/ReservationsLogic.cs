using System;
using System.Collections.Generic;
using System.Globalization;
using Project.DataModels;

public class ReservationsLogic
{
    private readonly ReservationsAccess _reservationsAccess = new ReservationsAccess();

    // vaste tijdsloten
    private static readonly List<string> ValidTimes = new List<string>
    {
        "17:00","17:30","18:00","18:30","19:00","19:30","20:00","20:30"
    };

    // ===== helpers =====
    private bool CheckGuestCount(int n)
    {
        return n >= 1 && n <= 6;
    }

    private bool CheckTime(string hhmm)
    {
        return ValidTimes.Contains(hhmm);
    }

    private bool TryParseExactDateTime(string s, out DateTime dt)
    {
        return DateTime.TryParseExact(
            s,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out dt
        );
    }

    private bool CheckStartAt(string startAt)
    {
        DateTime dt;
        if (!TryParseExactDateTime(startAt, out dt)) return false;
        if (dt <= DateTime.Now) return false;
        string hhmm = dt.ToString("HH:mm");
        return CheckTime(hhmm);
    }

    private bool IsOwner(int reservationId, int userId)
    {
        ReservationModel r = _reservationsAccess.GetById(reservationId);
        if (r == null) return false;
        return r.UserId == userId;
    }

    // ===== READ (gast: eigen lijst) =====
    public List<ReservationModel> GetReservationsByUserIdForGuest(int userId)
    {
        return _reservationsAccess.GetReservationsByUserIdSimple(userId);
    }

    // ===== UPDATE (gast: alleen eigen + formats) =====
    public bool UpdateReservationForGuest(int id, int userId, int guestCount, string startAt)
    {
        if (!IsOwner(id, userId)) return false;
        if (!CheckGuestCount(guestCount)) return false;
        if (!CheckStartAt(startAt)) return false;

        _reservationsAccess.UpdateReservationSimple(id, guestCount, startAt);
        return true;
    }

    // ===== DELETE (gast: alleen eigen) =====
    public bool DeleteReservationForGuest(int id, int userId)
    {
        if (!IsOwner(id, userId)) return false;
        _reservationsAccess.DeleteReservationSimple(id);
        return true;
    }

    // (handig voor bestaande aanroepen)
    public bool IsValidReservationDateTime(string input)
    {
        return CheckStartAt(input);
    }
}
