using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;
using OnlineDiaryApp.Services.Interfaces;

namespace OnlineDiaryApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var hash = HashPassword(password);
            return user.PasswordHash == hash ? user : null;
        }

        public async Task<User> RegisterAsync(string email, string username, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("Користувач з таким email вже існує");

            var user = new User
            {
                Email = email,
                Username = username,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return user;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public int? GetCurrentUserId(HttpContext httpContext)
        {
            var userIdString = httpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId)) return null;
            return userId;
        }
    }
}
