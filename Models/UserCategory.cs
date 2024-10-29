using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class UserCategory
    {
        public int UserCategoryId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
