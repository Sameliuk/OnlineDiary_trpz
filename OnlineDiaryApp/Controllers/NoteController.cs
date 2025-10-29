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
        private readonly FileService _fileService;
        private readonly NotebookService _notebookService;

        public NoteController(
            NoteService noteService,
            ReminderService reminderService,
            TagService tagService,
            FileService fileService,
            NotebookService notebookService)
        {
            _noteService = noteService;
            _reminderService = reminderService;
            _tagService = tagService;
            _fileService = fileService;
            _notebookService = notebookService;
        }

        // GET: /Note/
        public async Task<IActionResult> Index(string? sortBy, string? tag)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "User");

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

            var notes = await _noteService.GetAllNotesByUserAsync(userId.Value, strategy);
            ViewBag.SortBy = sortBy;
            ViewBag.SelectedTag = tag;
            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId.Value);
            ViewBag.Notebooks = await _notebookService.GetAllNotebooksAsync(userId.Value);

            return View(notes);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int notebookId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "User");

            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId.Value);
            ViewBag.NotebookId = notebookId;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(
    string title,
    string content,
    int notebookId, // 🔹 новий параметр
    List<int>? tagIds,
    DateTime? reminderDate,
    List<string>? GoogleDriveLinks,
    IFormFile? voiceNote)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            var notebook = await _notebookService.GetNotebookByIdAsync(notebookId);
            if (notebook == null)
                return BadRequest("Блокнот не знайдено");

            var note = await _noteService.CreateNoteAsync(title, content, userId, notebookId, tagIds ?? new List<int>());

            // Google Drive файли
            if (GoogleDriveLinks != null)
            {
                foreach (var link in GoogleDriveLinks)
                    await _fileService.AddLinkFileAsync(note.Id, "Google Drive file", link);
            }

            // Голосова нотатка
            if (voiceNote != null && voiceNote.Length > 0)
            {
                var fileName = Path.GetFileName(voiceNote.FileName);
                var filePath = Path.Combine("wwwroot", "VoiceNotes", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await voiceNote.CopyToAsync(stream);
                }

                await _fileService.AddVoiceFileAsync(note.Id, fileName, "/VoiceNotes/" + fileName);
            }

            await _noteService.UpdateNoteAsync(note, tagIds ?? new List<int>(), reminderDate);

            // 🔹 після створення перенаправляємо у список нотаток конкретного блокнота
            return RedirectToAction("ListByNotebook", new { notebookId });
        }

        public async Task<IActionResult> ListByNotebook(int notebookId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "User");

            var notes = await _noteService.GetNotesByNotebookAsync(notebookId);
            var notebook = await _notebookService.GetNotebookByIdAsync(notebookId);

            ViewBag.Notebook = notebook;
            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId.Value);

            return View("Index", notes);
        }



        // GET: /Note/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "User");

            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId.Value);
            ViewBag.Reminder = await _reminderService.GetReminderByNoteIdAsync(id);

            return View(note);
        }

        // POST: /Note/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            string title,
            string content,
            List<int>? tagIds,
            DateTime? remindAt,
            List<string>? GoogleDriveLinks,
            List<int>? DeletedFileIds,
             IFormFile? voiceNote)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            note.Title = title;
            note.Content = content;

            // Видалення файлів
            if (DeletedFileIds != null)
            {
                foreach (var fileId in DeletedFileIds)
                    await _fileService.DeleteFileAsync(fileId);
            }

            // Додавання нових файлів
            if (GoogleDriveLinks != null && GoogleDriveLinks.Any())
            {
                foreach (var link in GoogleDriveLinks)
                {
                    await _fileService.AddLinkFileAsync(note.Id, "Google Drive file", link);
                }
            }

            if (voiceNote != null && voiceNote.Length > 0)
            {
                var fileName = Path.GetFileName(voiceNote.FileName);
                var filePath = Path.Combine("wwwroot", "VoiceNotes", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await voiceNote.CopyToAsync(stream);
                }

                await _fileService.AddVoiceFileAsync(note.Id, fileName, "/VoiceNotes/" + fileName);
            }



            await _noteService.UpdateNoteAsync(note, tagIds ?? new List<int>(), remindAt);

            return RedirectToAction("Index");
        }

        // GET: /Note/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await _reminderService.GetReminderByNoteIdAsync(id);
            if (reminder != null)
                await _reminderService.DeleteReminderAsync(reminder.Id);

            await _noteService.DeleteNoteAsync(id);
            return RedirectToAction("Index");
        }

        // GET: /Note/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            ViewBag.Reminder = await _reminderService.GetReminderByNoteIdAsync(id);

            // Підвантажуємо файли для відображення
            note.Files = (await _fileService.GetFilesByNoteIdAsync(note.Id)).ToList();

            return View(note);
        }

        // GET: /Note/Search
        public async Task<IActionResult> Search(string keyword)
        {
            var userId = GetUserId();
            if (userId == null) return View(new List<Note>());

            var notes = await _noteService.SearchByTitleAsync(keyword);
            notes = notes.Where(n => n.UserId == userId.Value);
            return View("Index", notes);
        }

        // Допоміжний метод для отримання UserId
        private int? GetUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId)) return null;
            return userId;
        }
    }
}
