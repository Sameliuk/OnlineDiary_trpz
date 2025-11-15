using OnlineDiaryApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace OnlineDiaryApp.Patterns.Strategy
{
    public class SortByDateStrategy : ISortStrategy
    {
        public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
            notes.OrderByDescending(n => n.CreatedAt);
    }
}
