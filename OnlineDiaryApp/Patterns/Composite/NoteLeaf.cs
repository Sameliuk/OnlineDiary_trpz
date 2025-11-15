using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Patterns.Composite
{
    public class NoteLeaf : INotebookComponent
    {
        public Note Note { get; }

        public NoteLeaf(Note note)
        {
            Note = note;
        }

        public IEnumerable<INotebookComponent> GetChildren()
        {
            return new List<INotebookComponent>();
        }

        public void Display(int depth)
        {
            Console.WriteLine(new string('-', depth) + Note.Title);
        }
    }
}
