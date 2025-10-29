using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Composite
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
            System.Console.WriteLine(new string('-', depth) + Note.Title);
        }
    }
}
