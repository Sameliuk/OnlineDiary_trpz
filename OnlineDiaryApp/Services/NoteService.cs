using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineDiaryApp.Services
{
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

    public class NoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly ITagRepository _tagRepository;

        public NoteService(INoteRepository noteRepository, ITagRepository tagRepository)
        {
            _noteRepository = noteRepository;
            _tagRepository = tagRepository;
        }

        // Отримати всі нотатки конкретного користувача зі стратегією сортування
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

        public async Task<Note?> GetNoteByIdAsync(int id)
        {
            return await _noteRepository.GetByIdAsync(id);
        }

        public async Task<Note> CreateNoteAsync(string title, string content, int userId, List<int> tagIds)
        {
            var tags = new List<Tag>();
            foreach (var id in tagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag != null)
                    tags.Add(tag);
            }

            // Поточний час у UTC
            var utcNow = DateTime.UtcNow ;

            var note = new Note
            {
                Title = title,
                Content = content,
                UserId = userId,
                Tags = tags,
                CreatedAt = utcNow // зберігаємо у UTC
            };

            await _noteRepository.AddAsync(note);
            await _noteRepository.SaveChangesAsync();

            return note; // повертаємо створену нотатку
        }



        public async Task UpdateNoteAsync(Note note, List<int> tagIds)
        {
            var tags = new List<Tag>();
            foreach (var id in tagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag != null)
                    tags.Add(tag);
            }

            note.Tags = tags;
            await _noteRepository.UpdateAsync(note);
            await _noteRepository.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(int id)
        {
            await _noteRepository.DeleteAsync(id);
            await _noteRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Note>> SearchByTitleAsync(string keyword)
        {
            var notes = await _noteRepository.GetAllAsync();
            return notes.Where(n => n.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        // ✅ Новий метод: отримати всі теги
        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }
    }
}
