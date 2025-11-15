using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Utilities;
using OnlineDiaryApp.Patterns.Strategy;
using OnlineDiaryApp.Services.Interfaces;
using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;
        private readonly IReminderService _reminderService;
        private readonly ITagService _tagService;
        private readonly IFileService _fileService;
        private readonly INotebookService _notebookService;
        private readonly IUserService _userService;

        public NoteController(
            INoteService noteService,
            IReminderService reminderService,
            ITagService tagService,
            IFileService fileService,
            INotebookService notebookService,
            IUserService userService)
        {
            _noteService = noteService;
            _reminderService = reminderService;
            _tagService = tagService;
            _fileService = fileService;
            _notebookService = notebookService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? sortBy, string? tag)
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            ISortStrategy? strategy = sortBy?.ToLower() switch
            {
                "date" => new SortByDateStrategy(),
                "tag" when !string.IsNullOrEmpty(tag) => new SortByTagStrategy(tag),
                "title" => new SortByTitleStrategy(),
                _ => null
            };

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
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId.Value);
            ViewBag.NotebookId = notebookId;
            ViewBag.GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            ViewBag.GoogleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
        string title,
        string content,
        int notebookId,
        List<int>? tagIds,
        DateTime? reminderDate,
        List<string>? GoogleDriveLinks,
        List<string>? GoogleDriveNames,
        IFormFile? voiceNote)
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            var notebook = await _notebookService.GetNotebookByIdAsync(notebookId);
            if (notebook == null) return BadRequest("Блокнот не знайдено");

            var note = await _noteService.CreateNoteAsync(
                title,
                content,
                userId.Value,
                notebookId,
                tagIds ?? new List<int>()
            );

            if (GoogleDriveLinks != null && GoogleDriveNames != null)
            {
                for (int i = 0; i < GoogleDriveLinks.Count; i++)
                {
                    var link = GoogleDriveLinks[i];
                    var name = GoogleDriveNames.Count > i ? GoogleDriveNames[i] : "Google Drive file";
                    await _fileService.AddLinkFileAsync(note.Id, name, link);
                }
            }

            if (voiceNote != null)
                await _fileService.AddVoiceFileAsync(note.Id, voiceNote);

            DateTime? reminderUtc = null;

            if (reminderDate.HasValue)
            {
                var kyivTime = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
                reminderUtc = TimeZoneInfo.ConvertTimeToUtc(reminderDate.Value, kyivTime);

                await _reminderService.CreateReminderAsync(note.Id, userId.Value, reminderUtc.Value);
            }

            await _noteService.UpdateNoteAsync(note, tagIds ?? new List<int>(), reminderUtc);

            return RedirectToAction("Details", "Notebook", new { id = notebook.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            ViewBag.Tags = await _tagService.GetAllTagsAsync(userId.Value);
            ViewBag.Reminder = await _reminderService.GetReminderByNoteIdAsync(id);
            ViewBag.GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            ViewBag.GoogleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            string title,
            string content,
            List<int>? tagIds,
            DateTime? remindAt,
            List<string>? GoogleDriveLinks,
            List<string>? GoogleDriveNames,
            List<int>? DeletedFileIds,
            IFormFile? voiceNote)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            note.Title = title;
            note.Content = content;

            if (DeletedFileIds != null)
                foreach (var fileId in DeletedFileIds)
                    await _fileService.DeleteFileAsync(fileId);

            if (GoogleDriveLinks != null && GoogleDriveNames != null)
                for (int i = 0; i < GoogleDriveLinks.Count; i++)
                {
                    var link = GoogleDriveLinks[i];
                    var name = GoogleDriveNames.Count > i ? GoogleDriveNames[i] : "Google Drive file";
                    await _fileService.AddLinkFileAsync(note.Id, name, link);
                }

            if (voiceNote != null)
                await _fileService.AddVoiceFileAsync(note.Id, voiceNote);

            var userId = _userService.GetCurrentUserId(HttpContext);

            DateTime? remindAtUtc = null;
            if (remindAt.HasValue)
            {
                var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
                remindAtUtc = TimeZoneInfo.ConvertTimeToUtc(remindAt.Value, kyivTimeZone);

                await _reminderService.UpdateReminderAsync(note.Id, userId.Value, remindAtUtc.Value);
            }

            await _noteService.UpdateNoteAsync(note, tagIds ?? new List<int>(), remindAtUtc);

            return RedirectToAction("Details", "Notebook", new { id = note.NotebookId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await _reminderService.GetReminderByNoteIdAsync(id);
            if (reminder != null)
                await _reminderService.DeleteReminderAsync(reminder.Id);

            await _noteService.DeleteNoteAsync(id);
            return RedirectToAction("Index", "Notebook");
        }

        public async Task<IActionResult> Details(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            ViewBag.Reminder = await _reminderService.GetReminderByNoteIdAsync(id);
            note.Files = (await _fileService.GetFilesByNoteIdAsync(note.Id)).ToList();

            return View(note);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            var pdfBytes = PdfGenerator.GenerateNotePdf(note.Title, note.Content);
            var fileName = $"{note.Title}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
