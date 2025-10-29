using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
    }
}
