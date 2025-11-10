using Project.DataModels;
using System;
using System.Collections.Generic;

public class ReservationsLogic
{
    public static int? CurrentUserId { get; set; }

    private ReservationsAccess _reservationsAccess = new ReservationsAccess();

    private ReservationModel? GetOwnedReservation(int reservationId)
    {
        ReservationModel? r = _reservationsAccess.GetById(reservationId);
        if (r == null) return null;

        if (CurrentUserId.HasValue && r.UserId != CurrentUserId.Value)
            return null;

        return r;
    }

    public bool ReservationExistsForCurrentUser(int reservationId)
    {
        return GetOwnedReservation(reservationId) != null;
    }

    public List<ReservationModel> GetAllReservations() => _reservationsAccess.GetAll();

    public List<ReservationModel> GetReservationsByDate(string date) => _reservationsAccess.GetByDate(date);

    public List<ReservationModel> GetReservationsByDateRange(string startDate, string endDate)
        => _reservationsAccess.GetByDateRange(startDate, endDate);

    public List<ReservationModel> GetReservationsByUserId(int userId)
        => _reservationsAccess.GetByUserId(userId);

    public ReservationModel? GetReservationById(int id) => _reservationsAccess.GetById(id);

    public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Open")
    {
        if (guestCount < 1 || guestCount > 6)
            return false;

        ReservationModel reservation = new ReservationModel
        {
            UserId = userId,
            TableId = tableId,
            GuestCount = guestCount,
            StartAt = startAt,
            Status = status,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        _reservationsAccess.Write(reservation);
        return true;
    }

    public bool UpdateReservationStatus(int reservationId, string newStatus)
    {
        ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
        if (reservation == null) return false;

        reservation.Status = newStatus;
        reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        _reservationsAccess.Update(reservation);
        return true;
    }

    public bool DeleteReservation(int reservationId)
    {
        _reservationsAccess.DeleteById(reservationId);
        return true;
    }

    public static string FormatDateForDatabase(DateTime date)
    {
        return date.ToString("dd-MM-yyyy");
    }

    public static string FormatDateTimeForDatabase(DateTime dateTime)
    {
        return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
    }

    public static bool IsValidDateFormat(string dateString)
    {
        return DateTime.TryParseExact(
            dateString,
            "dd-MM-yyyy",
            null,
            System.Globalization.DateTimeStyles.None,
            out _);
    }

    public bool ChangeReservationTime(int reservationId, DateTime newTime)
    {
        if (newTime <= DateTime.Now)
            return false;

        ReservationModel? reservation = GetOwnedReservation(reservationId);
        if (reservation == null)
            return false;

        reservation.StartAt = newTime.ToString("dd-MM-yyyy HH:mm:ss");
        reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        _reservationsAccess.Update(reservation);
        return true;
    }

    public bool ChangeReservationPersons(int reservationId, int newGuestCount)
    {
        if (newGuestCount < 1 || newGuestCount > 6)
            return false;

        ReservationModel? reservation = GetOwnedReservation(reservationId);
        if (reservation == null)
            return false;

        if (reservation.TableCapacity >= newGuestCount)
        {
            reservation.GuestCount = newGuestCount;
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            _reservationsAccess.UpdateGuestCount(reservation);
            return true;
        }

        List<TableModel> availableTables = _reservationsAccess.GetFreeTables(reservation);
        if (availableTables.Count == 0)
            return false;

        TableModel newTable = availableTables[0];
        reservation.TableId = newTable.ID;
        reservation.GuestCount = newGuestCount;
        reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        _reservationsAccess.UpdateReservationTable(reservation);
        return true;
    }

    public bool CancelReservation(int reservationId)
    {
        ReservationModel? reservation = GetOwnedReservation(reservationId);
        if (reservation == null)
            return false;

        reservation.Status = "Geannuleerd";
        reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        _reservationsAccess.CancelReservation(reservation);
        return true;
    }

    public List<ReservationModel> GetReservationsByUserIdForGuest(int userId)
        => _reservationsAccess.GetReservationsByUserIdSimple(userId);

    public void UpdateReservationForGuest(int id, int guestCount, string startAt)
    {
        if (guestCount < 1 || guestCount > 6 || string.IsNullOrEmpty(startAt))
            return;

        ReservationModel? reservation = GetOwnedReservation(id);
        if (reservation == null)
            return;

        _reservationsAccess.UpdateReservationSimple(id, guestCount, startAt);
    }

    public void DeleteReservationForGuest(int id)
    {
        ReservationModel? reservation = GetOwnedReservation(id);
        if (reservation == null)
            return;

        _reservationsAccess.DeleteReservationSimple(id);
    }

    public bool IsValidReservationDateTime(string input)
    {
        if (!DateTime.TryParse(input, out DateTime dateTime))
            return false;

        return dateTime.Hour >= 17;
    }

    public string SelectArrivalTime()
    {
        List<string> timeSlots = new List<string>
        {
            "17:00","17:30","18:00","18:30",
            "19:00","19:30","20:00","20:30"
        };

        int selectedIndex = 0;
        ConsoleKey key = ConsoleKey.NoName;

        while (key != ConsoleKey.Enter)
        {
            Console.Clear();
            Console.WriteLine("=== Reservations ===");
            Console.WriteLine("Select arrival time:");

            for (int i = 0; i < timeSlots.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("> ");
                    Console.Write(timeSlots[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("  ");
                    Console.Write(timeSlots[i]);
                }
                Console.WriteLine();
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
                selectedIndex = selectedIndex == 0 ? timeSlots.Count - 1 : selectedIndex - 1;
            else if (key == ConsoleKey.DownArrow)
                selectedIndex = (selectedIndex + 1) % timeSlots.Count;
        }

        return timeSlots[selectedIndex];
    }
}
