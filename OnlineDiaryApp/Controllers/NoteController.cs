using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;

namespace OnlineDiaryApp.Controllers
{
    public class NoteController : Controller
    {
        private readonly NoteService _noteService;
        private readonly ReminderService _reminderService;
        private readonly TagService _tagService;

        public NoteController(NoteService noteService, ReminderService reminderService, TagService tagService)
        {
            _noteService = noteService;
            _reminderService = reminderService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index(string? sortBy, string? tag)
        {

            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToAction("Login", "User");

            ISortStrategy? strategy = null;
            if (!string.IsNullOrEmpty(sortBy))
            {
                strategy = sortBy.ToLower() switch
                {
                    "date" => new SortByDateStrategy(),
                    "tag" when !string.IsNullOrEmpty(tag) => new SortByTagStrategy(tag),
                    "title" => new SortByTitleStrategy(),
                    _ => null
                };
            }

            var notes = await _noteService.GetAllNotesByUserAsync(userId, strategy);

            ViewBag.SortBy = sortBy;
            ViewBag.SelectedTag = tag;
            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId);

            return View(notes);
        }

        public async Task<IActionResult> Create()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToAction("Login", "User"); 

            var tags = await _tagService.GetAllTagsAsync(userId);
            ViewBag.Tags = tags ?? new List<Tag>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string title, string content, List<int>? tagIds, DateTime? reminderDate)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToAction("Index");

            var note = await _noteService.CreateNoteAsync(title, content, userId, tagIds ?? new List<int>());

            if (reminderDate.HasValue)
            {
                await _reminderService.CreateReminderAsync(note.Id, reminderDate.Value, userId);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
                return NotFound();

            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToAction("Login", "User"); 

            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId) ?? new List<Tag>();

            var reminder = await _reminderService.GetReminderByNoteIdAsync(id);
            ViewBag.Reminder = reminder;

            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string title, string content, List<int>? tagIds, DateTime? reminderDate)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
                return NotFound();

            note.Title = title;
            note.Content = content;

            await _noteService.UpdateNoteAsync(note, tagIds ?? new List<int>());

            var existingReminder = await _reminderService.GetReminderByNoteIdAsync(note.Id);
            if (reminderDate.HasValue)
            {
                if (existingReminder != null)
                {
                    await _reminderService.UpdateReminderAsync(existingReminder, reminderDate.Value);
                }
                else
                {
                    await _reminderService.CreateReminderAsync(note.Id, reminderDate.Value, note.UserId);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _noteService.DeleteNoteAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return View(new List<Note>());

            var notes = await _noteService.SearchByTitleAsync(keyword);
            notes = notes.Where(n => n.UserId == userId);
            return View("Index", notes);
        }
    }
}
