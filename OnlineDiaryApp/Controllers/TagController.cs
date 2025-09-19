using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Services;
using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Controllers
{
    public class TagController : Controller
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        // Список тегів
        public async Task<IActionResult> Index()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return View(tags);
        }

        // GET: створення
        public IActionResult Create()
        {
            return View();
        }

        // POST: створення
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Назва тегу обов’язкова");
                return View();
            }

            await _tagService.CreateTagAsync(name);
            return RedirectToAction("Index");
        }

        // Видалення тегу
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteTagAsync(id);
            return RedirectToAction("Index");
        }
    }
}
