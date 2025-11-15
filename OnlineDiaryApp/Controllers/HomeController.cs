using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services.Interfaces;

namespace OnlineDiaryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly INoteService _noteService;
        private readonly IUserService _userService;

        public HomeController(INoteService noteService, IUserService userService)
        {
            _noteService = noteService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue)
                return RedirectToAction("Login", "User");

            var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            var today = TimeZoneInfo.ConvertTime(DateTime.Now, kyivTimeZone);

            ViewBag.Today = today;
            ViewBag.Month = today.Month;
            ViewBag.Year = today.Year;

            var allNotes = await _noteService.GetAllNotesByUserAsync(userId.Value);

            return View(allNotes);
        }
    }
}
