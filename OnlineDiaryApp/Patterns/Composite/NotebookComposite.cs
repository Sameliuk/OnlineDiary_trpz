using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Patterns.Composite
{
    public class NotebookComposite : INotebookComponent
    {
        private readonly List<INotebookComponent> _children = new List<INotebookComponent>();

        public Notebook Notebook { get; }

        public NotebookComposite(Notebook notebook)
        {
            Notebook = notebook;
        }

        public void Add(INotebookComponent component)
        {
            _children.Add(component);
        }

        public void Remove(INotebookComponent component)
        {
            _children.Remove(component);
        }

        public IEnumerable<INotebookComponent> GetChildren()
        {
            return _children;
        }

        public void Display(int depth)
        {
            Console.WriteLine(new string('-', depth) + Notebook.Name);
            foreach (var child in _children)
            {
                child.Display(depth + 2);
            }
        }

        public string Name => Notebook.Name;
        public int Id => Notebook.Id;
    }
}
