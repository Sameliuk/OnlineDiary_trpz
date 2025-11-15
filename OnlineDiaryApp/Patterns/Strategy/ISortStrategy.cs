using OnlineDiaryApp.Models;
using System.Collections.Generic;

namespace OnlineDiaryApp.Patterns.Strategy
{
    public interface ISortStrategy
    {
        IEnumerable<Note> Sort(IEnumerable<Note> notes);
    }
}
