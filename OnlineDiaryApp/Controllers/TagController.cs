using Microsoft.AspNetCore.Mvc;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;
using OnlineDiaryApp.Services.Interfaces;

namespace OnlineDiaryApp.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly IUserService _userService;

        public TagController(ITagService tagService, IUserService userService)
        {
            _tagService = tagService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            var tags = await _tagService.GetAllTagsAsync(userId.Value);
            return View(tags);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var userId = _userService.GetCurrentUserId(HttpContext);
            if (!userId.HasValue) return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Назва тегу обов’язкова");
                return View();
            }

            await _tagService.CreateTagAsync(name, userId.Value);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteTagAsync(id);
            return RedirectToAction("Index");
        }
    }
}
