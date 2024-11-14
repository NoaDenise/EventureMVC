using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models
{
    public class User 
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Förnamn måste vara mellan 1-50 tecken")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Förnamn måste vara mellan 1-50 tecken")]
        public string LastName { get; set; }
        public string? UserLocation { get; set; }
        public bool IsAdmin { get; set; }
        public string PhoneNumber { get; set; }

    }
}
