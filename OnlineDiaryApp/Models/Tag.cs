using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineDiaryApp.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
