using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly UniversityContext _context;

        public EnrollmentsController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            var universityContext = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);
            return View(await universityContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["Course"] = new SelectList(_context.Course, "Id", "Title");
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName");
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> EnrollStudents()
        {
            

            var enrollment = _context.Enrollment.Include(s => s.Student).Include(s=>s.Course).First();

            if(enrollment == null)
            {
                return NotFound();
            }

            var enrollStudentsVM = new EnrollStudentsViewModel
            {
                Enrollment = enrollment,
                CoursesList = new SelectList(_context.Course.OrderBy(m=>m.Title),"Id","Title"),
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.Id), "Id", "FullName"),
                SelectedStudents = enrollment.Course.Students.Select(sa => sa.StudentId),
            };

            return View(enrollStudentsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EnrollStudents(EnrollStudentsViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.Enrollment);
                    await _context.SaveChangesAsync();


                    int id = model.CourseId;
                    IEnumerable<int> listStudents = model.SelectedStudents;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId ==id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                    foreach (int studentId in newStudents)
                        _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id });

                    int Year = model.Year;
                    string Semester = model.Semester;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(model.Enrollment.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

            }
            return View(model);
        }



        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,CourseId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,CourseId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
