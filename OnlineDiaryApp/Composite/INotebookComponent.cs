namespace OnlineDiaryApp.Composite
{
    public interface INotebookComponent
    {
        void Display(int depth);                    
        IEnumerable<INotebookComponent> GetChildren();
    }
}
