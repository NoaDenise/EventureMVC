namespace EventureMVC.Models.ViewModel
{
    public class CommentViewModel
    {
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }
        public int CommentId { get; set; }
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
