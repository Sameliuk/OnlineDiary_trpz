using OnlineDiaryApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace OnlineDiaryApp.Patterns.Strategy
{
    public class SortByTagStrategy : ISortStrategy
    {
        private readonly string _tag;

        public SortByTagStrategy(string tag) => _tag = tag;

        public IEnumerable<Note> Sort(IEnumerable<Note> notes) =>
            notes.Where(n => n.Tags.Any(t => t.Name == _tag));
    }
}
