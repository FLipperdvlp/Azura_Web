using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student.Data;
using student.Models;

namespace student.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ApplicationDbContext context, ILogger<StudentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Class)
                .OrderBy(s => s.LastName)
                .ToListAsync();

            return View(students);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Class)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();
            return View(student);
        }

        public async Task<IActionResult> Create()
        {
            await PrepareClassesMultiSelectAsync();
            return View(new Student { EnrollmentDate = DateTime.UtcNow.Date });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, int[] selectedClasses)
        {
            if (ModelState.IsValid)
            {
                foreach (var classId in selectedClasses.Distinct())
                {
                    student.Enrollments.Add(new Enrollment { ClassId = classId });
                }

                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PrepareClassesMultiSelectAsync(selectedClasses);
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            var selected = student.Enrollments.Select(e => e.ClassId).ToArray();
            await PrepareClassesMultiSelectAsync(selected);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student formModel, int[] selectedClasses)
        {
            if (id != formModel.Id) return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            if (ModelState.IsValid)
            {
                student.FirstName = formModel.FirstName;
                student.LastName = formModel.LastName;
                student.EnrollmentDate = formModel.EnrollmentDate;

                var selectedSet = selectedClasses.ToHashSet();
                student.Enrollments.RemoveWhere(e => !selectedSet.Contains(e.ClassId));
                foreach (var classId in selectedSet)
                {
                    if (!student.Enrollments.Any(e => e.ClassId == classId))
                        student.Enrollments.Add(new Enrollment { ClassId = classId, StudentId = student.Id });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PrepareClassesMultiSelectAsync(selectedClasses);
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PrepareClassesMultiSelectAsync(int[]? selected = null)
        {
            var classes = await _context.Classes.OrderBy(c => c.Title).ToListAsync();
            ViewBag.Classes = classes;
            ViewBag.SelectedClassIds = selected ?? Array.Empty<int>();
        }
    }

    internal static class CollectionExtensions
    {
        public static void RemoveWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            var items = source.Where(predicate).ToList();
            foreach (var i in items) source.Remove(i);
        }
    }
}