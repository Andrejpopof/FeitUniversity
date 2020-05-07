using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class CoursesController : Controller
    {
        private readonly UniversityContext _context;

        public CoursesController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string courseProgramm , string searchString,int? courseSemester)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();
            IQueryable<string> ProgrammQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            IQueryable<int> SemesterQuery = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();



            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(courseProgramm))
            {
                courses = courses.Where(x => x.Programme == courseProgramm);
            }
            if(courseSemester!=null)
            {
                courses = courses.Where(x => x.Semester == courseSemester);
            }

            courses = courses.Include(m => m.FirstTeacher).Include(m => m.SecondTeacher).Include(m => m.Students).ThenInclude(m => m.Student);

            var courseTitleVM = new CourseTitleViewModel 
            {
                Programms = new SelectList(await ProgrammQuery.ToListAsync()),
                Semesters = new SelectList(await SemesterQuery.ToListAsync()),
                Courses = await courses.ToListAsync()
            };

            return View(courseTitleVM);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Include(c=>c.Students).ThenInclude(c=>c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }




        //GET:  Courses/TeachersCourses/1
        public async Task<IActionResult> TeachersCourses(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            var teachers = _context.Teacher.Where(m => m.Id == id).FirstOrDefault();

            var courses = _context.Course.Where(s => s.FirstTeacherId == id || s.SecondTeacherId == id);

            return View(courses);
        }


        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
