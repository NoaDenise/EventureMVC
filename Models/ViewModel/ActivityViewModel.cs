using EventureMVC.Models.ViewModel;

namespace EventureMVC.Models
{
    public class ActivityViewModel
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? DateOfActivity { get; set; }
        public string ActivityLocation { get; set; }
        public string? ImageUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ContactInfo { get; set; }

        public List<CommentViewModel> Comments { get; set; } /*= new List<CommentViewModel>();*/


    }
}
