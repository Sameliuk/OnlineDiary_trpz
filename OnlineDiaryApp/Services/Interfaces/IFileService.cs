namespace OnlineDiaryApp.Services.Interfaces
{
    public interface IFileService
    {
        Task AddLinkFileAsync(int noteId, string fileName, string fileUrl);
        Task AddVoiceFileAsync(int noteId, IFormFile voiceNote);
        Task<IEnumerable<NoteFile>> GetFilesByNoteIdAsync(int noteId);
        Task DeleteFileAsync(int fileId);
    }
}
