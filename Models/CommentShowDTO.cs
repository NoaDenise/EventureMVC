namespace EventureMVC.Models
{
    public class CommentShowDTO
    {
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }
        public int CommentId { get; set; }
       
    }
}
