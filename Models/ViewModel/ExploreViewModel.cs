namespace EventureMVC.Models.ViewModel
{
    public class ExploreViewModel
    {
        public List<Activity> Activities { get; set; }
        public Dictionary<string, List<string>> CountriesWithCities { get; set; }

        // Properties for filters
        public bool IsFree { get; set; }
        public bool Is18Plus { get; set; }
        public bool IsFamilyFriendly { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }

        public List<int> LikedActivities { get; set; } // List of liked activity IDs
    }
}
