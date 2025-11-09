using OnlineDiaryApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace OnlineDiaryApp.Patterns.Strategy
{
    public class SortByTitleStrategy : ISortStrategy
    {
        public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
            notes.OrderBy(n => n.Title);
    }
}
