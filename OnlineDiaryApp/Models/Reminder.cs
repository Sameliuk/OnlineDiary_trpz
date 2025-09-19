namespace OnlineDiaryApp.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public DateTime RemindAt { get; set; }
        public string Status { get; set; } = "active";

        public int UserId { get; set; }
        public User User { get; set; }

        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
