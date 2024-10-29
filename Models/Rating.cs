using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class Rating
    {
        public int RatingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [Range(1, 5)]
        public int? Score { get; set; }
    }
}
