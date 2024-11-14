using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventureMVC.Models.ViewModel
{
    public class AdminInformationViewModel
    {
        //public string Id { get; set; }
        [Required] 
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
