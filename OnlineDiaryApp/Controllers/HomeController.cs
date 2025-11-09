using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services;

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
            var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            var today = TimeZoneInfo.ConvertTime(DateTime.Now, kyivTimeZone);

            ViewBag.Today = today;
            ViewBag.Month = today.Month;
            ViewBag.Year = today.Year;

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                int.TryParse(userIdClaim.Value, out userId);
            }

            var allNotes = await _noteService.GetAllNotesAsync();

            var userNotes = allNotes.Where(n => n.UserId == userId);

            return View(userNotes);
        }
    }
}
