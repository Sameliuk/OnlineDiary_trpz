using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class FileService
    {
        private readonly INoteFileRepository _noteFileRepository;
        private readonly string _voiceNotesFolder = Path.Combine("wwwroot", "VoiceNotes");

        public FileService(INoteFileRepository noteFileRepository)
        {
            _noteFileRepository = noteFileRepository;
            if (!Directory.Exists(_voiceNotesFolder))
                Directory.CreateDirectory(_voiceNotesFolder);
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

            await _noteFileRepository.AddAsync(noteFile);
            await _noteFileRepository.SaveChangesAsync();
        }

        public async Task AddVoiceFileAsync(int noteId, IFormFile voiceNote)
        {
            if (voiceNote == null || voiceNote.Length == 0) return;

            var savedFilePath = await SaveVoiceFileAsync(voiceNote);
            var noteFile = new NoteFile
            {
                NoteId = noteId,
                FileName = Path.GetFileName(savedFilePath),
                FilePath = "/" + Path.GetRelativePath("wwwroot", savedFilePath).Replace("\\", "/"),
                MimeType = "audio/wav"
            };

            await _noteFileRepository.AddAsync(noteFile);
            await _noteFileRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<NoteFile>> GetFilesByNoteIdAsync(int noteId)
        {
            return await _noteFileRepository.GetFilesByNoteIdAsync(noteId);
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = await _noteFileRepository.GetByIdAsync(fileId);
            if (file == null) return;

            var fullPath = Path.Combine("wwwroot", file.FilePath.TrimStart('/').Replace("/", "\\"));
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            await _noteFileRepository.DeleteAsync(fileId);
            await _noteFileRepository.SaveChangesAsync();
        }

        private async Task<string> SaveVoiceFileAsync(IFormFile voiceNote)
        {
            var fileName = Path.GetFileName(voiceNote.FileName);
            var filePath = Path.Combine(_voiceNotesFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await voiceNote.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
