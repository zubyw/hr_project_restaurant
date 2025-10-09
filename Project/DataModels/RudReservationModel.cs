namespace Project.DataModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public int GuestCount { get; set; }
        public string StartAt { get; set; }
        public string Status { get; set; }
        public string CanModifyUntil { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}

