using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services;

namespace OnlineDiaryApp.Controllers
{
    public class ReminderController : Controller
    {
        private readonly ReminderService _reminderService;

        public ReminderController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        public IActionResult Create(int noteId)
        {
            ViewBag.NoteId = noteId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int noteId, DateTime remindAt)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            await _reminderService.CreateReminderAsync(noteId, remindAt, userId);

            return RedirectToAction("Details", "Note", new { id = noteId });
        }
    }
}
