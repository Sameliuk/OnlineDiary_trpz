namespace OnlineDiaryApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Notebook> Notebooks { get; set; } = new List<Notebook>();

    }
}
