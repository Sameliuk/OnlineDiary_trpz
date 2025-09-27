using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;

namespace OnlineDiaryApp.Controllers
{
    public class TagController : Controller
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToAction("Login", "User"); 

            var tags = await _tagService.GetAllTagsAsync(userId);
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Назва тегу обов’язкова");
                return View();
            }

            await _tagService.CreateTagAsync(name, userId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteTagAsync(id);
            return RedirectToAction("Index");
        }

    }
}
