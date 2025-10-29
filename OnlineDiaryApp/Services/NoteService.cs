using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Services
    {
        // Стратегії сортування
        public interface ISortStrategy
        {
            IEnumerable<Note> Sort(IEnumerable<Note> notes);
        }

        public class SortByDateStrategy : ISortStrategy
        {
            public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
                notes.OrderByDescending(n => n.CreatedAt);
        }

        public class SortByTagStrategy : ISortStrategy
        {
            private readonly string _tag;
            public SortByTagStrategy(string tag) => _tag = tag;

            public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
                notes.Where(n => n.Tags.Any(t => t.Name == _tag));
        }

        public class SortByTitleStrategy : ISortStrategy
        {
            public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
                notes.OrderBy(n => n.Title);
        }

        // Основний сервіс для нотаток
        public class NoteService
        {
            private readonly INoteRepository _noteRepository;
            private readonly ITagRepository _tagRepository;
            private readonly INotebookRepository _notebookRepository;

            private readonly ReminderService _reminderService;

            public NoteService(
                INoteRepository noteRepository,
                ITagRepository tagRepository,
                INotebookRepository notebookRepository,

                ReminderService reminderService)
            {
                _noteRepository = noteRepository;
                _tagRepository = tagRepository;
                _notebookRepository = notebookRepository;

                _reminderService = reminderService;
            }

            // Отримати всі нотатки користувача із сортуванням
            public async Task<IEnumerable<Note>> GetAllNotesByUserAsync(int userId, ISortStrategy? strategy = null)
            {
                var notes = (await _noteRepository.GetAllAsync())
                            .Where(n => n.UserId == userId);

                return strategy != null ? strategy.Sort(notes) : notes;
            }

            public async Task<IEnumerable<Note>> GetAllNotesAsync(ISortStrategy? strategy = null)
            {
                var notes = await _noteRepository.GetAllAsync();
                return strategy != null ? strategy.Sort(notes) : notes;
            }

            // Отримати нотатку за Id з тегами та файлами
            public async Task<Note?> GetNoteByIdAsync(int id)
            {
                var note = await _noteRepository.GetByIdWithTagsAndFilesAsync(id);
                return note;
            }

            // Створення нотатки
            public async Task<Note> CreateNoteAsync(string title, string content, int userId, int notebookId, List<int> tagIds)
            {
                var notebook = await _notebookRepository.GetByIdAsync(notebookId);
                if (notebook == null)
                    throw new Exception("Блокнот із вказаним ID не знайдено.");

                var tags = new List<Tag>();
                foreach (var id in tagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(id);
                    if (tag != null)
                        tags.Add(tag);
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


            // Оновлення нотатки
            public async Task UpdateNoteAsync(Note note, List<int> tagIds, DateTime? remindAt = null)
            {
                var tags = new List<Tag>();
                foreach (var id in tagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(id);
                    if (tag != null)
                        tags.Add(tag);
                }

                note.Tags = tags;
                note.UpdatedAt = DateTime.UtcNow;

                await _noteRepository.UpdateAsync(note);
                await _noteRepository.SaveChangesAsync();

                // Оновлення або видалення нагадування
                var reminder = await _reminderService.GetReminderByNoteIdAsync(note.Id);

                if (remindAt.HasValue)
                {
                    if (reminder != null)
                        await _reminderService.UpdateReminderAsync(reminder, remindAt.Value);
                    else
                        await _reminderService.CreateReminderAsync(note.Id, remindAt.Value, note.UserId);
                }
                else if (reminder != null)
                {
                    await _reminderService.DeleteReminderAsync(reminder.Id);
                }
            }

            // Видалення нотатки
            public async Task DeleteNoteAsync(int id)
            {
                await _noteRepository.DeleteAsync(id);
                await _noteRepository.SaveChangesAsync();
            }

            // Пошук нотаток за заголовком
            public async Task<IEnumerable<Note>> SearchByTitleAsync(string keyword)
            {
                var notes = await _noteRepository.GetAllAsync();
                return notes.Where(n => n.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            // Отримати всі теги користувача
            public async Task<IEnumerable<Tag>> GetAllTagsAsync(int userId)
            {
                var tags = await _tagRepository.GetAllAsync(userId);
                return tags.Where(t => t.UserId == userId);
            }

            public async Task<IEnumerable<Note>> GetNotesByNotebookAsync(int notebookId)
            {
                return await _noteRepository.GetNotesByNotebookIdAsync(notebookId);
            }


        }
    }

