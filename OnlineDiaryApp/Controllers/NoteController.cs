using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: /Note
        public async Task<IActionResult> Index(string? sortBy, string? tag)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return View(new List<Note>());

            ISortStrategy? strategy = null;
            if (!string.IsNullOrEmpty(sortBy))
            {
                strategy = sortBy.ToLower() switch
                {
                    "date" => new SortByDateStrategy(),
                    "tag" when !string.IsNullOrEmpty(tag) => new SortByTagStrategy(tag),
                    _ => null
                };
            }

            var notes = await _noteService.GetAllNotesByUserAsync(userId, strategy);
            return View(notes);
        }

        // GET: /Note/Create
        public async Task<IActionResult> Create()
        {
            var tags = await _tagService.GetAllTagsAsync();
            ViewBag.Tags = tags ?? new List<Tag>();
            return View();
        }

        // POST: /Note/Create
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

        // GET: /Note/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
                return NotFound();

            ViewBag.Tags = await _tagService.GetAllTagsAsync();

            var reminder = await _reminderService.GetReminderByNoteIdAsync(id);
            ViewBag.Reminder = reminder; // передаємо існуюче нагадування у View

            return View(note);
        }

        // POST: /Note/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string title, string content, List<int>? tagIds, DateTime? reminderDate)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
                return NotFound();

            // Оновлюємо поля нотатки
            note.Title = title;
            note.Content = content;

            // Оновлюємо теги
            await _noteService.UpdateNoteAsync(note, tagIds ?? new List<int>());

            // Обробка нагадування
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
            // Не видаляємо нагадування, якщо користувач нічого не ввів




            return RedirectToAction("Index");
        }


        // GET: /Note/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            await _noteService.DeleteNoteAsync(id);
            return RedirectToAction("Index");
        }

        // GET: /Note/Search?keyword=...
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
