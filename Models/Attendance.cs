using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public string UserId { get; set; }

        public int ActivityId { get; set; }

        public bool? IsAttending { get; set; }
    }
}
