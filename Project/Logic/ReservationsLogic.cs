using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;

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

            ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
            if (reservation == null)
            {
                return false;
            }

            string newTimeString = newTime.ToString("yyyy-MM-dd HH:mm:ss");

            // Check if the current table is free at the new time
            if (!_reservationsAccess.IsTableFree(newTimeString, reservation.TableId, reservationId))
            {
                return false; // Table is not available at this time
            }

            // Use the existing UpdateReservationTime method
            return _reservationsAccess.UpdateReservationTime(reservationId, newTimeString);
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

            ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
            if (reservation == null)
            {
                return false;
            }

            // Check if current table can accommodate the new guest count
            if (reservation.TableCapacity >= newGuestCount)
            {
                // Current table is fine, just update guest count
                reservation.GuestCount = newGuestCount;
                reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _reservationsAccess.Update(reservation);
                return true;
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
        catch
        {
            return false;
        }
    }

    public bool CancelReservation(int reservationId)
    {
        try
        {
            // Use the existing CancelReservation method in ReservationsAccess
            return _reservationsAccess.CancelReservation(reservationId);
        }
        catch
        {
            return false;
        }
    }

}
