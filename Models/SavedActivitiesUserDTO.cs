namespace EventureMVC.Models
{
    public class SavedActivitiesUserDTO
    {
        public int UserEventId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityLocation { get; set; }
        public DateTime? DateOfActivity { get; set; }
    }
}
