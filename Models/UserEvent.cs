using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class UserEvent
    {
        public int UserEventId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ActivityId { get; set; }
    }
}
