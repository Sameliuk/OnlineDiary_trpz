using OnlineDiaryApp.Data;

namespace OnlineDiaryApp.Services
{
    public class FileService
    {
        private readonly AppDbContext _context;

        public FileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddLinkFileAsync(int noteId, string fileName, string fileUrl)
        {
            var noteFile = new NoteFile
            {
                NoteId = noteId,
                FileName = fileName,
                FilePath = fileUrl,
                MimeType = "link/google-drive"
            };

            _context.NoteFiles.Add(noteFile);
            await _context.SaveChangesAsync();
        }

        public async Task AddVoiceFileAsync(int noteId, string fileName, string fileUrl)
        {
            var noteFile = new NoteFile
            {
                NoteId = noteId,
                FileName = fileName,
                FilePath = fileUrl,
                MimeType = "audio/wav" // або інший формат, наприклад "audio/mp3"
            };

            _context.NoteFiles.Add(noteFile);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NoteFile>> GetFilesByNoteIdAsync(int noteId)
        {
            return _context.NoteFiles.Where(f => f.NoteId == noteId).ToList();
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = await _context.NoteFiles.FindAsync(fileId);
            if (file == null) return;

            _context.NoteFiles.Remove(file);
            await _context.SaveChangesAsync();
        }
    }
}
