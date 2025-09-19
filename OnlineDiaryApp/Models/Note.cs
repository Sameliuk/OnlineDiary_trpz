namespace OnlineDiaryApp.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public Reminder? Reminder { get; set; }
    }
}
