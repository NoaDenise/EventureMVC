namespace EventureMVC.Models
{
    public class UserAttendanceDTO
    {
        public int AttendanceId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityLocation { get; set; }
        public DateTime? DateOfActivity { get; set; }
    }
}
