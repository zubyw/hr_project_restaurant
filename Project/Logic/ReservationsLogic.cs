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
        ReservationModel reservation = new ReservationModel
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


    public bool ChangeReservationTime(int reservationId, DateTime newTime)
    {
        if (newTime <= DateTime.Now)
        {
            return false;
        }

        ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
        if (reservation == null)
        {
            return false;
        }

        reservation.StartAt = newTime.ToString("yyyy-MM-dd HH:mm:ss");
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _reservationsAccess.Update(reservation);
        return true;
    }

    public bool ChangeReservationPersons(int reservationId, int newGuestCount)
    {
        if (newGuestCount != 2 && newGuestCount != 4 && newGuestCount != 6)
        {
            return false;
        }

        ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
        if (reservation == null)
        {
            return false;
        }

        // Check if current table can accommodate the new guest count
        if (reservation.TableCapacity >= newGuestCount)
        {
            // Current table is fine, just update guest count
            return _reservationsAccess.UpdateGuestCount(reservationId, newGuestCount);
        }
        else
        {
            // Need to find a bigger table
            var availableTables = _reservationsAccess.GetFreeTablesExcluding(reservation.StartAt, newGuestCount, reservationId);
            if (!availableTables.Any())
            {
                return false; // No suitable table available
            }

            // Use the UpdateReservationTable method which updates both table and guest count
            return _reservationsAccess.UpdateReservationTable(reservationId, availableTables.First().ID, newGuestCount);
        }
    }

    public bool CancelReservation(int reservationId)
    {
        ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
        if (reservation == null)
        {
            return false;
        }

        reservation.Status = "Geannuleerd";
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _reservationsAccess.Update(reservation);
        return true;
    }

}
