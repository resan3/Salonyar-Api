namespace ApiSalonyar.Models
{
    public partial class StaffSchedule
    {
        public int ScheduleId { get; set; }
        public int StaffId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public bool IsWorkDay { get; set; } = true;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
