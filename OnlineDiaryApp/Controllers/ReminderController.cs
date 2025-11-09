using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services;

namespace OnlineDiaryApp.Controllers
{
    public class ReminderController : Controller
    {
        private readonly ReminderService _reminderService;
        private readonly UserService _userService;

        public ReminderController(ReminderService reminderService, UserService userService)
        {
            _reminderService = reminderService;
            _userService = userService;
        }

        public IActionResult Create(int noteId)
        {
            ViewBag.NoteId = noteId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int noteId, DateTime remindAt)
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            await _reminderService.CreateReminderAsync(noteId, remindAt, userId.Value);

            return RedirectToAction("Details", "Note", new { id = noteId });
        }
    }
}
