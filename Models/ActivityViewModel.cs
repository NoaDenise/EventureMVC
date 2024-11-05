namespace EventureMVC.Models
{
    public class ActivityViewModel
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? DateOfActivity { get; set; }
        public string ActivityLocation { get; set; }
        public string? ImageUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ContactInfo { get; set; }

    }
}
