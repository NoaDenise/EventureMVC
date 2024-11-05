using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class CommentCreateEditDTO
    {
        [StringLength(2500)]
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }    
        public int ActivityId { get; set; }
    }
}
