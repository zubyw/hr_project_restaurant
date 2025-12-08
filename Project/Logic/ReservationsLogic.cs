using Project.DataAccess;
using Project.DataModels;
using Project.Logic;
using System;
using System.Collections.Generic;

namespace Project.Logic
{
    public class ReservationsLogic
    {
        // Set the current logged-in user (after login)
        public static int? CurrentUserId { get; set; }

        private ReservationsAccess _reservationsAccess = new ReservationsAccess();
        private DishAccess _dishAccess = new DishAccess();

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
        public bool CreateReservation(int userId, int tableId, int guestCount, string startAt, string status = "Open")
        {
            if (guestCount < 1 || guestCount > 6)
                return false;

            ReservationModel reservation = new ReservationModel();
            reservation.UserId = userId;
            reservation.TableId = tableId;
            reservation.GuestCount = guestCount;
            reservation.StartAt = startAt;
            reservation.Status = status;
            reservation.CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            _reservationsAccess.Write(reservation);
            return true;
        }

        // Update the status of a reservation (admin/staff or when reservation is updated use)
        public bool UpdateReservationStatus(ReservationModel reservation, string newStatus = "Canceled")
        {
            if (reservation == null) return false;

            reservation.Status = newStatus;
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            _reservationsAccess.Update(reservation);
            return true;
        }

        // Delete a reservation (admin/staff path; no ownership check here by design)
        public bool DeleteReservation(int reservationId)
        {
            _reservationsAccess.DeleteById(reservationId);
            return true;
        }

        // Format helper: only date (dd-MM-yyyy)
        public static string FormatDateForDatabase(DateTime date)
        {
            return date.ToString("dd-MM-yyyy");
        }

        // Format helper: date and time (dd-MM-yyyy HH:mm:ss)
        public static string FormatDateTimeForDatabase(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }

        // Check if date format is valid
        public static bool IsValidDateFormat(string dateString)
        {
            return DateTime.TryParseExact(
                dateString,
                "dd-MM-yyyy",
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

            reservation.StartAt = newTime.ToString("dd-MM-yyyy HH:mm:ss");
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

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
                reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
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
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            _reservationsAccess.UpdateReservationTable(reservation);
            return true;
        }

        public bool CancelReservation(int reservationId)
        {
            ReservationModel? reservation = GetOwnedReservation(reservationId);
            if (reservation == null)
                return false;

            reservation.Status = "Canceled";
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            _reservationsAccess.CancelReservation(reservation);
            return true;
        }

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

        public bool ReservationContainsDishes(ReservationModel reservation)
        {
            return _reservationsAccess.DoesReservationHaveDishes(reservation);
        }

        public void UpdateGuestCountForReservation(ReservationModel reservation, int newGuestCount)
        {
            reservation.GuestCount = newGuestCount;
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            _reservationsAccess.UpdateReservationGuestCount(reservation);
        }



        public void DeleteDishesFromReservation(ReservationModel reservation)
        {
            if (ReservationContainsDishes(reservation))
            {
                _dishAccess.DeleteDishesOnReservation(reservation);
            }
        }

        public int GetAvailableGuestCount(ReservationModel reservation)
        {
            switch (reservation.GuestCount)
            {
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 3:
                    return 4;
                case 4:
                    return 3;
                case 5:
                    return 6;
                case 6:
                    return 5;
            }
            return 0;
        }
        public ReservationModel? ReloadReservation(ReservationModel reservation)
        {
            return _reservationsAccess.GetReservationByIdSimple(reservation.ID);
        }

        public void UpdateTableForReservation(ReservationModel reservation)
        {
            _reservationsAccess.UpdateReservationTable(reservation);
        }


        public void UpdateDateTimeForReservation(ReservationModel reservation)
        {
            _reservationsAccess.UpdateReservationDateTime(reservation);
        }


        public bool IsReservationCanceled(ReservationModel reservation)
        {
            string reservationStatus = _reservationsAccess.GetReservationStatus(reservation);
            if (reservationStatus == "Canceled")
            {
                return true;
            }
            return false;
        }


        public List<(string DishName, int Count)> GetDishCountsForDate(string date)
        {
            return _reservationsAccess.GetDishCountsByDate(date);
        }
    }
}
    


