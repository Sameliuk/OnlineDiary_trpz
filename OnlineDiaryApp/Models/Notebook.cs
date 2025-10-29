namespace OnlineDiaryApp.Models
{
    public class Notebook
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
