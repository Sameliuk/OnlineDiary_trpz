using OnlineDiaryApp.Patterns.Composite;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;

namespace OnlineDiaryApp.Patterns.Composite
{
    // Будівельник для побудови структури з сервісів
    public class NotebookStructureBuilder
    {
        private readonly NotebookService _notebookService;
        private readonly NoteService _noteService;

        public NotebookStructureBuilder(NotebookService notebookService, NoteService noteService)
        {
            _notebookService = notebookService;
            _noteService = noteService;
        }

        public async Task<INotebookComponent> BuildNotebookTreeAsync(int userId)
        {
            var root = new NotebookComposite(new Notebook
            {
                Name = "Усі блокноти користувача"
            });

            var notebooks = await _notebookService.GetAllNotebooksAsync(userId);

            foreach (var notebook in notebooks)
            {
                var notebookComposite = new NotebookComposite(notebook);
                var notes = await _noteService.GetNotesByNotebookAsync(notebook.Id);

                foreach (var note in notes)
                {
                    notebookComposite.Add(new NoteLeaf(note));
                }

                root.Add(notebookComposite);
            }

            return root;
        }
    }
}
