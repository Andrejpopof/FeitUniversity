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
        public async Task<IActionResult> Index(string courseTitle,string semestar , int? godina)
        {
            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();

            IQueryable<string> SemesterQuery = _context.Enrollment.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<int> GodinaQuery = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();


            if(!string.IsNullOrEmpty(courseTitle))
            {
                enrollments = enrollments.Where(m => m.Course.Title.Contains(courseTitle));
            }

            if(!string.IsNullOrEmpty(semestar))
            {
                enrollments = enrollments.Where(m => m.Semester.Contains(semestar));
            }
            if(godina!=null)
            {
                enrollments = enrollments.Where(m => m.Year == godina);

            }
            enrollments = enrollments.Include(m => m.Student).Include(m => m.Course);


            var enrollmentSearchVM = new EnrollmentsSearchViewModelcs
            {
                Enrollments = await enrollments.ToListAsync(),
                Semester = new SelectList(await SemesterQuery.ToListAsync())

            };

            return View(enrollmentSearchVM);
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
        public async Task<IActionResult> EnrollStudents(int? id)
        {

            var course = _context.Course.Where(s => s.Id == id).Include(s => s.Students).First();

            if (course == null)
            {
                return NotFound();
            }

            var enrollStudentsVM = new EnrollStudentsViewModel
            {
                Course = course,
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.Id), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId),
            };

            return View(enrollStudentsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EnrollStudents(int id , EnrollStudentsViewModel model )
        {
            if(ModelState.IsValid)
            {
                try { 
                IEnumerable<int> listStudents = model.SelectedStudents;
                IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
                _context.Enrollment.RemoveRange(toBeRemoved);
                IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));

                foreach (int studentId in newStudents)
                    _context.Enrollment.Add(new Enrollment{ StudentId = studentId,CourseId = id,Year = model.Year,Semester=model.Semester,});

                await _context.SaveChangesAsync();
            }
                catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(CoursesController.Index));
        }
            return View(model);
        }

        public async Task<IActionResult> UnEnrollStudents(int? id)
        {
            var course = _context.Course.Where(s => s.Id == id).Include(s => s.Students).First();

            if (course == null)
            {
                return NotFound();
            }

            var enrollStudentsVM = new EnrollStudentsViewModel
            {
                Course = course,
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.Id), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId),
            };

            return View(enrollStudentsVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnEnrollStudents(int id, EnrollStudentsViewModel model) //NEZNAM KAKO UNENROLL
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IEnumerable<int> listStudents = model.SelectedStudents;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);
                    IQueryable<Enrollment> toBeUnEnrolled = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id);
                    IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));

                    foreach (int studentId in existStudents)
                        _context.Enrollment.UpdateRange(toBeUnEnrolled);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(CoursesController.Index));
            }
            return View(model);
        }


        public async Task<IActionResult>Uvid(int id)
        {
            if ( id == null)
            {
                return NotFound();
            }

            TempData["selectedCourse"] = id;
            TempData.Keep();

            var courses = _context.Course.Where(s => s.Id == id).FirstOrDefault();
            ViewData["CourseTitle"]=courses.Title;
             var enrollments = _context.Enrollment.Where(m => m.CourseId == id).Include(m=>m.Student);
             return View(enrollments);
        }


        [HttpGet]
        public async Task<IActionResult> EditStudentStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string kurs;
            if(TempData["selectedCourse"]!=null)
            {
                kurs = TempData["selectedCourse"].ToString();
            }
            TempData.Keep();

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentStatus(int id, [Bind("Id,StudentId,CourseId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }
            int kursId=0;
            string kurs = null;
            if (TempData["selectedCourse"] != null)
            {
                kursId = Int32.Parse(TempData["selectedCourse"].ToString());
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
                return RedirectToAction("Uvid", "Enrollments", new { id = kursId });

            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        public async Task<IActionResult> StudentsCourses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var students = _context.Student.Where(s => s.Id == id).FirstOrDefault();
            ViewData["imeStudent"] = students.FullName;

            TempData["student"] = id.ToString();

            var enrollments = _context.Enrollment.Where(s => s.StudentId == id).Include(s=>s.Course);
            return View(enrollments);
        }

        [HttpGet]
        public async Task<IActionResult> UvidStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string kurs=null;
            if (TempData["student"] != null)
            {
                kurs = TempData["student"].ToString();
            }
            TempData.Keep();

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UvidStudent(int id, [Bind("Id,StudentId,CourseId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }
            int kursId = 0;
            if (TempData["student"] != null)
            {
                kursId = Int32.Parse(TempData["student"].ToString());
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
                return RedirectToAction("StudentsCourses", "Enrollments", new { id = kursId });

            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
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
