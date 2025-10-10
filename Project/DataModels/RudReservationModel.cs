namespace Project.DataModels
{
    public class RudReservationModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public int GuestCount { get; set; }
        public string StartAt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CanModifyUntil { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
}

