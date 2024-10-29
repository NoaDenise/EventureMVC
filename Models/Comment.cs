using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [StringLength(2500)]
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ActivityId { get; set; }
    }
}
