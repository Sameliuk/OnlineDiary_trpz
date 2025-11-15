using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace OnlineDiaryApp.Controllers
{
    public class ReminderController : Controller
    {
        private readonly IReminderService _reminderService;
        private readonly IUserService _userService;

        public ReminderController(IReminderService reminderService, IUserService userService)
        {
            _reminderService = reminderService;
            _userService = userService;
        }

        [HttpGet]
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

            var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            var utcRemindAt = TimeZoneInfo.ConvertTimeToUtc(remindAt, kyivTimeZone);

            await _reminderService.CreateReminderAsync(noteId, userId.Value, utcRemindAt);

            return RedirectToAction("Details", "Note", new { id = noteId });
        }
    }
}
