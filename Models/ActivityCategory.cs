using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class ActivityCategory
    {
        public int ActivityCategoryId { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
