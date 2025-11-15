using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;
using OnlineDiaryApp.Patterns.Strategy;
using OnlineDiaryApp.Services.Interfaces;

namespace OnlineDiaryApp.Services.Implementations
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly ITagRepository _tagRepository;
        private readonly INotebookRepository _notebookRepository;
        private readonly IReminderRepository _reminderRepository;

        public NoteService(
            INoteRepository noteRepository,
            ITagRepository tagRepository,
            INotebookRepository notebookRepository,
            IReminderRepository reminderRepository)
        {
            _noteRepository = noteRepository;
            _tagRepository = tagRepository;
            _notebookRepository = notebookRepository;
            _reminderRepository = reminderRepository;
        }

        public async Task<IEnumerable<Note>> GetAllNotesByUserAsync(int userId, ISortStrategy? strategy = null)
        {
            var notes = (await _noteRepository.GetAllAsync())
                .Where(n => n.UserId == userId);

            return strategy?.Sort(notes) ?? notes;
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync(ISortStrategy? strategy = null)
        {
            var notes = await _noteRepository.GetAllAsync();
            return strategy?.Sort(notes) ?? notes;
        }

        public async Task<Note?> GetNoteByIdAsync(int id) =>
            await _noteRepository.GetByIdWithTagsAndFilesAsync(id);

        public async Task<Note> CreateNoteAsync(string title, string content, int userId, int notebookId, List<int> tagIds)
        {
            var notebook = await _notebookRepository.GetByIdAsync(notebookId);
            if (notebook == null)
                throw new Exception("Блокнот із вказаним ID не знайдено.");

            var tags = new List<Tag>();
            foreach (var id in tagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag != null) tags.Add(tag);
            }

            var note = new Note
            {
                Title = title,
                Content = content,
                UserId = userId,
                NotebookId = notebookId,
                Tags = tags,
                CreatedAt = DateTime.UtcNow
            };

            await _noteRepository.AddAsync(note);
            await _noteRepository.SaveChangesAsync();

            return note;
        }

        public async Task UpdateNoteAsync(Note note, List<int> tagIds, DateTime? remindAt = null)
        {
            var tags = new List<Tag>();
            foreach (var id in tagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag != null) tags.Add(tag);
            }
            note.Tags = tags;
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(note);
            await _noteRepository.SaveChangesAsync();

            if (remindAt.HasValue)
            {
                var reminder = await _reminderRepository.GetByNoteIdAsync(note.Id);

                var utcRemindAt = DateTime.SpecifyKind(remindAt.Value, DateTimeKind.Utc);

                if (reminder == null)
                {
                    reminder = new Reminder
                    {
                        NoteId = note.Id,
                        UserId = note.UserId,
                        RemindAt = utcRemindAt,
                        Status = "active"
                    };
                    await _reminderRepository.AddAsync(reminder);
                }
                else
                {
                    reminder.RemindAt = utcRemindAt;
                    reminder.Status = "active";
                    await _reminderRepository.UpdateAsync(reminder);
                }

                await _reminderRepository.SaveChangesAsync();
            }
            else
            {
                var reminder = await _reminderRepository.GetByNoteIdAsync(note.Id);
                if (reminder != null)
                {
                    await _reminderRepository.DeleteAsync(reminder.Id);
                    await _reminderRepository.SaveChangesAsync();
                }
            }

        }

        public async Task DeleteNoteAsync(int id)
        {
            await _noteRepository.DeleteAsync(id);
            await _noteRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Note>> SearchByTitleAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Note>();

            var notes = await _noteRepository.GetAllAsync();
            return notes.Where(n => n.Title != null &&
                                    n.Title.ToLower().Contains(keyword.ToLower()))
                        .ToList();
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(int userId)
        {
            var tags = await _tagRepository.GetAllAsync(userId);
            return tags.Where(t => t.UserId == userId);
        }

        public async Task<IEnumerable<Note>> GetNotesByNotebookAsync(int notebookId) =>
            await _noteRepository.GetNotesByNotebookIdAsync(notebookId);
    }
}
