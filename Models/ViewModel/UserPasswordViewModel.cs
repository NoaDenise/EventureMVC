using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models.ViewModel
{
    public class UserPasswordViewModel
    {
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password has to be at least 8 characters, including a special character, 2 numbers and 1 upper case letter.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = " The passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
