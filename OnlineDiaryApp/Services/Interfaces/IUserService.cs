using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Services.Interfaces
{
    public interface IUserService
    {
        
        Task<User?> AuthenticateAsync(string email, string password);

        Task<User> RegisterAsync(string email, string username, string password);

        int? GetCurrentUserId(HttpContext httpContext);
    }
}
