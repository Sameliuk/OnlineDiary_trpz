using OnlineDiaryApp.Models;

public class NoteFile
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? MimeType { get; set; }

    public Note Note { get; set; } = null!;
}
