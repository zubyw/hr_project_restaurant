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

        // Check whether reservation can be modified or canceled (must be >=24 hours from now)
        public bool CanModifyOrCancel(ReservationModel reservation)
        {
            if (reservation == null) return false;
            DateTime dt;
            if (!DateTime.TryParseExact(reservation.StartAt, "dd-MM-yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dt))
                return false;
            return (dt - DateTime.Now) >= TimeSpan.FromHours(24);
        }

        // --- Reads ---

        // Get all reservations from database
        public List<ReservationModel> GetAllReservations()
        {
            List<ReservationModel> res = _reservationsAccess.GetAll();

            string nowSortable = ToSortableString(DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

            return res
                .Where(r => string.Compare(ToSortableString(r.StartAt), nowSortable) >= 0)
                .OrderBy(r => ToSortableString(r.StartAt))
                .ToList();
        }

        // Get all reservations from one user
        public List<ReservationModel> GetReservationsByUserId(int userId)
        {
            return _reservationsAccess.GetByUserId(userId);
        }

        // Create a new reservation (only if between 1 and 6 guests)
        public bool CreateReservation(ReservationModel reservation)
        {
            if (reservation.GuestCount < 1 || reservation.GuestCount > 6)
                return false;

            _reservationsAccess.Write(reservation);
            return true;
        }

        public bool CancelReservation(int reservationId)
        {
            ReservationModel? reservation = GetOwnedReservation(reservationId);
            if (reservation == null)
                return false;
            if (!CanModifyOrCancel(reservation))
                return false;
            reservation.Status = "Canceled";
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            _reservationsAccess.CancelReservation(reservation);
            return true;
        }

        public List<ReservationModel> GetReservationsByUserIdForGuest(int userId)
        {
            List<ReservationModel> res =  _reservationsAccess.GetReservationsByUserIdSimple(userId);
            string nowSortable = ToSortableString(DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

            return res
                .Where(r => string.Compare(ToSortableString(r.StartAt), nowSortable) >= 0)
                .OrderBy(r => ToSortableString(r.StartAt))
                .ToList();
        }

        private static string ToSortableString(string startAt)
        {
            // only works for datetime string in format -> dd-MM-yyyy HH:MM.
            string year  = startAt.Substring(6, 4);
            string month = startAt.Substring(3, 2);
            string day   = startAt.Substring(0, 2);
            string hour  = startAt.Substring(11, 2);
            string minute= startAt.Substring(14, 2);

            string final = year + month + day + hour + minute;
            return final;
        }

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

        public int GetAllowedGuestCountAtUpdate(ReservationModel reservation)
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
        public void UpdateReservation(ReservationModel reservation)
        {
            reservation.UpdatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            _reservationsAccess.Update(reservation);
        }

        

    }
}
    


