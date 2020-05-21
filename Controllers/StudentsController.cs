using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityContext _context;
        private object webHostEnvironment;

        private IWebHostEnvironment WebHostEnvironment { get; }

        public StudentsController(UniversityContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            webHostEnvironment = WebHostEnvironment;
        }

        // GET: Students
        public async Task<IActionResult> Index(string firstName,string lastName)
        {
            IQueryable<Student> students = _context.Student.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
            {
                students = students.Where(m => m.FirstName.Contains(firstName));
            }
            if(!string.IsNullOrEmpty(lastName))
            {
                students = students.Where(m => m.LastName.Contains(lastName));
            }
            
            students=students.Include(m => m.Courses).ThenInclude(m => m.Course);

            var StudentNameMV = new StudentImePrezimeModelView
            {
                Students = await students.ToListAsync()

            };
            return View(StudentNameMV);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(p=>p.Courses)
                .ThenInclude(p=>p.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemester,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(s => s.Id == id).Include(s => s.Courses).First();
            if (student == null)
            {
                return NotFound();
            }
            StudentCourseEdit viewModel = new StudentCourseEdit
            {
                Student = student,
                CourseList = new MultiSelectList(_context.Course.OrderBy(s => s.Title), "Id", "Title"),
                SelectedCourses = student.Courses.Select(sa => sa.CourseId)
            }; 
            return View(viewModel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentCourseEdit model)
        {
            if (id != model.Student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.Student);
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listCourses = model.SelectedCourses;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listCourses.Contains(s.CourseId) && s.StudentId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<int> existCourses = _context.Enrollment.Where(s => listCourses.Contains(s.CourseId) && s.StudentId == id).Select(s => s.CourseId);
                    IEnumerable<int> newCourses = listCourses.Where(s => !existCourses.Contains(s));
                    foreach (int courseId in newCourses)
                        _context.Enrollment.Add(new Enrollment { CourseId = courseId, StudentId = id });

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(model.Student.Id))
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

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }

        public IActionResult UploadPic(int id )
        {
            var viewModel = new StudentPictureViewModel {
                ProfileImage = null,
                student = null,

            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPic(int id, IFormFile file)
        {
            var viewModel = new StudentPictureViewModel
            {
                student = await _context.Student.FindAsync(id),
                ProfileImage = file,

            };

            string uniqueFileName = UploadedFile(viewModel.ProfileImage);
            viewModel.student.ProfilePicture = uniqueFileName;
            _context.Update(viewModel.student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private string UploadedFile(IFormFile model)
        {
            string uniqueFileName = null;

            if (model != null)
            {
                string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


    }
}
