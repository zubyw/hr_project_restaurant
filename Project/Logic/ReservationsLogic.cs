using System;
using System.Collections.Generic;
using Project.DataModels;

public class ReservationsLogic
{
    private ReservationsAccess _reservationsAccess = new ReservationsAccess();

    // Create reservation
    public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Pending")
    {
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

    // Cancel (admin)
    public bool CancelReservation(int reservationId)
    {
        ReservationModel res = _reservationsAccess.GetById(reservationId);
        if (res == null) return false;

        res.Status = "Geannuleerd";
        res.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _reservationsAccess.CancelReservation(res);
        return true;
    }

    // Update status (admin)
    public bool UpdateReservationStatus(int reservationId, string newStatus)
    {
        ReservationModel res = _reservationsAccess.GetById(reservationId);
        if (res == null) return false;

        res.Status = newStatus;
        res.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _reservationsAccess.Update(res);
        return true;
    }

    // Basic get methods
    public List<ReservationModel> GetAllReservations()
    {
        return _reservationsAccess.GetAll();
    }

    public List<ReservationModel> GetReservationsByUserId(int userId)
    {
        return _reservationsAccess.GetByUserId(userId);
    }

    public ReservationModel? GetReservationById(int id)
    {
        return _reservationsAccess.GetById(id);
    }

    public List<ReservationModel> GetReservationsByUserIdForGuest(int userId)
    {
        return _reservationsAccess.GetReservationsByUserIdSimple(userId);
    }
}
