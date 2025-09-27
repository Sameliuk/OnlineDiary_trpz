using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;


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

    public class SortByTitleStrategy : ISortStrategy
    {
        public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
            notes.OrderBy(n => n.Title);
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

        public async Task<Note?> GetNoteByIdAsync(int id) =>
            await _noteRepository.GetByIdAsync(id);

        public async Task<Note> CreateNoteAsync(string title, string content, int userId, List<int> tagIds)
        {
            var tags = new List<Tag>();
            foreach (var id in tagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag != null)
                    tags.Add(tag);
            }

            var utcNow = DateTime.UtcNow;

            var note = new Note
            {
                Title = title,
                Content = content,
                UserId = userId,
                Tags = tags,
                CreatedAt = utcNow
            };

            await _noteRepository.AddAsync(note);
            await _noteRepository.SaveChangesAsync();

            return note;
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

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(int userId) =>
    (await _tagRepository.GetAllAsync(userId)).Where(t => t.UserId == userId);


    }
}
