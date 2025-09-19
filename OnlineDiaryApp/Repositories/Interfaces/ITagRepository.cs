using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task AddAsync(Tag tag);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}



