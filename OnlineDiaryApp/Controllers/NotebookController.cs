using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services;
using OnlineDiaryApp.Patterns.Composite;
using OnlineDiaryApp.Patterns.Strategy;

namespace OnlineDiaryApp.Controllers
{
    public class NotebookController : Controller
    {
        private readonly NotebookService _notebookService;
        private readonly NoteService _noteService;
        private readonly TagService _tagService;
        private readonly UserService _userService;

        public NotebookController(NotebookService notebookService, NoteService noteService, TagService tagService, UserService userService)
        {
            _notebookService = notebookService;
            _noteService = noteService;
            _tagService = tagService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            var notebooks = await _notebookService.GetAllNotebooksAsync(userId.Value);
            return View(notebooks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string? description)
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            await _notebookService.CreateNotebookAsync(name, userId.Value, description);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var notebook = await _notebookService.GetNotebookByIdAsync(id);
            if (notebook == null) return NotFound();
            return View(notebook);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, string? description)
        {
            await _notebookService.UpdateNotebookAsync(id, name, description);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _notebookService.DeleteNotebookAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Notes(int notebookId)
        {
            var notebook = await _notebookService.GetNotebookByIdAsync(notebookId);
            if (notebook == null) return NotFound();

            var notes = await _noteService.GetNotesByNotebookAsync(notebook.Id);
            ViewBag.Notebook = notebook;
            return View(notes);
        }

        public async Task<IActionResult> Details(int id, string? sortBy, string? tag, string? keyword)
        {
            var notebook = await _notebookService.GetNotebookByIdAsync(id);
            if (notebook == null) return NotFound();

            var notes = await _noteService.GetNotesByNotebookAsync(id);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var q = keyword.ToLower();
                notes = notes.Where(n => !string.IsNullOrEmpty(n.Title) && n.Title.ToLower().Contains(q)).ToList();
            }

            if (!string.IsNullOrEmpty(tag))
                notes = notes.Where(n => n.Tags.Any(t => t.Name == tag)).ToList();

            ISortStrategy? strategy = sortBy?.ToLower() switch
            {
                "date" => new SortByDateStrategy(),
                "title" => new SortByTitleStrategy(),
                "tag" when !string.IsNullOrEmpty(tag) => new SortByTagStrategy(tag),
                _ => null
            };

            if (strategy != null)
                notes = strategy.Sort(notes).ToList();

            var notebookComposite = new NotebookComposite(notebook);
            foreach (var note in notes)
                notebookComposite.Add(new NoteLeaf(note));

            return View(notebookComposite);
        }
    }
}
