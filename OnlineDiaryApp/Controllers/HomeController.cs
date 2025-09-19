using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineDiaryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly NoteService _noteService;

        public HomeController(NoteService noteService)
        {
            _noteService = noteService;
        }

        public async Task<IActionResult> Index()
        {
            // Безпечне отримання Id користувача зі Claims
            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                int.TryParse(userIdClaim.Value, out userId);
            }

            // Якщо користувач не аутентифікований, userId = 0
            // Отримуємо всі нотатки
            var allNotes = await _noteService.GetAllNotesAsync();

            // Фільтруємо нотатки конкретного користувача
            var userNotes = allNotes.Where(n => n.UserId == userId);

            // Передаємо список нотаток у в’юшку
            return View(userNotes);
        }
    }
}
