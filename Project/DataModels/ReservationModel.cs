using System;

namespace Project.DataModels
{
    public class ReservationModel
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public int GuestCount { get; set; }
        public string StartAt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CanModifyUntil { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;

        public string GuestFirstName { get; set; } = string.Empty;
        public string GuestLastName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public int TableNumber { get; set; }
        public int TableCapacity { get; set; }
    }
}
