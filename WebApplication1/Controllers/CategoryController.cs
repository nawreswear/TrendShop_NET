using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repository;

        // Constructor injection of the repository
        public CategoryController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        // GET: Category
        [AllowAnonymous]
        public IActionResult Index()
        {
            var categories = _repository.GetAll();
            return View(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCategoriesPartial()
        {
            var categories = _repository.GetAll();
            return PartialView("_CategoriesMenu", categories);
        }

        // GET: Category/Details/5
        public IActionResult Details(int id)
        {
            var category = _repository.GetById(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _repository.GetById(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _repository.Update(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _repository.GetById(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
