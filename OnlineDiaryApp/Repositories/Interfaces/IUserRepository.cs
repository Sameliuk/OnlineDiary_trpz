using OnlineDiaryApp.Models;
using System.Threading.Tasks;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
