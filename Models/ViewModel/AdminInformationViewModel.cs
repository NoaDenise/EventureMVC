using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models.ViewModel
{
    public class AdminInformationViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name has to be between 1-50 characters")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name has to be between 1-50 characters")]
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        //dubbelkolla krav
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password has to be at least 8 characters, including a special character, 2 numbers and 1 upper case letter.")]
        [DataType (DataType.Password)]
        public string NewPassWord { get; set; }

        [DataType (DataType.Password)]
        [Compare("NewPassWord", ErrorMessage = " The passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
