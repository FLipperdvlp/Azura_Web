using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student.Data;
using student.Models;

namespace student.Controllers
{
    public class ClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Classes
                .Include(c => c.Enrollments)
                .OrderBy(c => c.Title)
                .ToListAsync();
            return View(list);
        }

        public IActionResult Create() => View(new Class());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class model)
        {
            if (!ModelState.IsValid) return View(model);

            model.Code = model.Code?.Trim();
            model.Title = model.Title?.Trim();

            var codeExists = await _context.Classes.AnyAsync(c => c.Code == model.Code);
            if (codeExists)
            {
                ModelState.AddModelError(nameof(Class.Code), "Класс с таким кодом уже существует.");
                return View(model);
            }

            _context.Classes.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null) return NotFound();
            return View(cls);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Class model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var cls = await _context.Classes
                .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cls == null) return NotFound();
            return View(cls);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null) return NotFound();
            return View(cls);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls != null)
            {
                _context.Classes.Remove(cls);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}