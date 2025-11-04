using Project.DataModels;
using System;
using System.Collections.Generic;

public class ReservationsLogic
{
    // Set the current logged-in user (after login)
    public static int? CurrentUserId { get; set; }

    private ReservationsAccess _reservationsAccess = new ReservationsAccess();

    // --- Helpers ---

    // Returns reservation only if it exists and belongs to CurrentUser; otherwise null
    private ReservationModel? GetOwnedReservation(int reservationId)
    {
        ReservationModel? r = _reservationsAccess.GetById(reservationId);
        if (r == null) return null;

        if (CurrentUserId.HasValue && r.UserId != CurrentUserId.Value)
            return null;

        return r;
    }

    // Public check for presentation layer (optional to use there)
    public bool ReservationExistsForCurrentUser(int reservationId)
    {
        return GetOwnedReservation(reservationId) != null;
    }

    // --- Reads ---

    // Get all reservations from database
    public List<ReservationModel> GetAllReservations()
    {
        return _reservationsAccess.GetAll();
    }

    // Get reservations by specific date
    public List<ReservationModel> GetReservationsByDate(string date)
    {
        return _reservationsAccess.GetByDate(date);
    }

    // Get reservations between two dates
    public List<ReservationModel> GetReservationsByDateRange(string startDate, string endDate)
    {
        return _reservationsAccess.GetByDateRange(startDate, endDate);
    }

    // Get all reservations from one user
    public List<ReservationModel> GetReservationsByUserId(int userId)
    {
        return _reservationsAccess.GetByUserId(userId);
    }

    // Get one reservation by ID (raw; no ownership check)
    public ReservationModel? GetReservationById(int id)
    {
        return _reservationsAccess.GetById(id);
    }

    // --- Create/Update/Delete ---

    // Create a new reservation (only if between 1 and 6 guests)
    public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Pending")
    {
        if (guestCount < 1 || guestCount > 6)
            return false;

        ReservationModel reservation = new ReservationModel();
        reservation.UserId = userId;
        reservation.TableId = tableId;
        reservation.GuestCount = guestCount;
        reservation.StartAt = startAt;
        reservation.Status = status;
        reservation.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _reservationsAccess.Write(reservation);
        return true;
    }

    // Update the status of a reservation (admin/staff use)
    public bool UpdateReservationStatus(int reservationId, string newStatus)
    {
        ReservationModel? reservation = _reservationsAccess.GetById(reservationId);
        if (reservation == null) return false;

        reservation.Status = newStatus;
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _reservationsAccess.Update(reservation);
        return true;
    }

    // Delete a reservation (admin/staff path; no ownership check here by design)
    public bool DeleteReservation(int reservationId)
    {
        _reservationsAccess.DeleteById(reservationId);
        return true;
    }

    // Format helper: only date (yyyy-MM-dd)
    public static string FormatDateForDatabase(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    // Format helper: date and time (yyyy-MM-dd HH:mm:ss)
    public static string FormatDateTimeForDatabase(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // Check if date format is valid
    public static bool IsValidDateFormat(string dateString)
    {
        return DateTime.TryParseExact(
            dateString,
            "yyyy-MM-dd",
            null,
            System.Globalization.DateTimeStyles.None,
            out _);
    }

    // --- Guest-facing changes (ownership enforced) ---

    // Change reservation time (only for your own reservation)
    public bool ChangeReservationTime(int reservationId, DateTime newTime)
    {
        if (newTime <= DateTime.Now)
            return false;

        ReservationModel? reservation = GetOwnedReservation(reservationId);
        if (reservation == null)
            return false;

        reservation.StartAt = newTime.ToString("yyyy-MM-dd HH:mm:ss");
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _reservationsAccess.Update(reservation);
        return true;
    }

    // Change number of guests (only for your own reservation)
    public bool ChangeReservationPersons(int reservationId, int newGuestCount)
    {
        if (newGuestCount < 1 || newGuestCount > 6)
            return false;

        ReservationModel? reservation = GetOwnedReservation(reservationId);
        if (reservation == null)
            return false;

        // If current table fits new guest count
        if (reservation.TableCapacity >= newGuestCount)
        {
            reservation.GuestCount = newGuestCount;
            reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _reservationsAccess.UpdateGuestCount(reservation);
            return true;
        }

        // Otherwise find another free table
        List<TableModel> availableTables = _reservationsAccess.GetFreeTables(reservation);
        if (availableTables.Count == 0)
            return false;

        TableModel newTable = availableTables[0];
        reservation.TableId = newTable.ID;
        reservation.GuestCount = newGuestCount;
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _reservationsAccess.UpdateReservationTable(reservation);
        return true;
    }

    // Cancel reservation (only for your own reservation)
    public bool CancelReservation(int reservationId)
    {
        ReservationModel? reservation = GetOwnedReservation(reservationId);
        if (reservation == null)
            return false;

        reservation.Status = "Geannuleerd";
        reservation.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _reservationsAccess.CancelReservation(reservation);
        return true;
    }

    // --- Guest simple API ---

    // Get guest's reservations (simplified version)
    public List<ReservationModel> GetReservationsByUserIdForGuest(int userId)
    {
        return _reservationsAccess.GetReservationsByUserIdSimple(userId);
    }

    // Update guest reservation (only if it's your own)
    public void UpdateReservationForGuest(int id, int guestCount, string startAt)
    {
        if (guestCount < 1 || guestCount > 6)
            return;

        if (string.IsNullOrEmpty(startAt))
            return;

        ReservationModel? reservation = GetOwnedReservation(id);
        if (reservation == null)
            return;

        _reservationsAccess.UpdateReservationSimple(id, guestCount, startAt);
    }

    // Delete guest reservation (only own)
    public void DeleteReservationForGuest(int id)
    {
        ReservationModel? reservation = GetOwnedReservation(id);
        if (reservation == null)
            return;

        _reservationsAccess.DeleteReservationSimple(id);
    }

    // Check if given time is valid (restaurant opens after 17:00)
    public bool IsValidReservationDateTime(string input)
    {
        DateTime dateTime;
        if (!DateTime.TryParse(input, out dateTime))
            return false;

        int hour = dateTime.Hour;
        return hour >= 17;
    }

    // Simple console selection for arrival time (presentation can call if needed)
    public string SelectArrivalTime()
    {
        List<string> timeSlots = new List<string>();
        timeSlots.Add("17:00");
        timeSlots.Add("17:30");
        timeSlots.Add("18:00");
        timeSlots.Add("18:30");
        timeSlots.Add("19:00");
        timeSlots.Add("19:30");
        timeSlots.Add("20:00");
        timeSlots.Add("20:30");

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
            {
                if (selectedIndex == 0) selectedIndex = timeSlots.Count - 1;
                else selectedIndex = selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % timeSlots.Count;
            }
        }

        return timeSlots[selectedIndex];
    }
}
