using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UniversityContext _context;
        private object webHostEnvironment;
        private IWebHostEnvironment WebHostEnvironment { get; }


        public TeachersController(UniversityContext context)
        {
            _context = context;
            webHostEnvironment = WebHostEnvironment;


        }

        // GET: Teachers
        public async Task<IActionResult> Index(string firstName,string lastName,string obrazovanie,string akademskiRank)
        {
            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();
            IQueryable<string> ObrazovanieQuery = _context.Teacher.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();
            IQueryable<string> AkademskiRankQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();

            if(!string.IsNullOrEmpty(firstName))
            {
                teachers = teachers.Where(t => t.FirstName.Contains(firstName));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                teachers = teachers.Where(t => t.LastName.Contains(lastName));
            }
            if (!string.IsNullOrEmpty(obrazovanie))
            {
                teachers = teachers.Where(t => t.Degree.Contains(obrazovanie));
            }
            if (!string.IsNullOrEmpty(akademskiRank))
            {
                teachers = teachers.Where(t => t.AcademicRank.Contains(akademskiRank));
            }

            var TeacherFilterVm = new NastavnikFilterModelView
            {
                Obrazovanie = new SelectList(await ObrazovanieQuery.ToListAsync()),
                AkademskiRank = new SelectList(await AkademskiRankQuery.ToListAsync()),
                Teachers = await teachers.ToListAsync()


            };



            return View(TeacherFilterVm);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }

        public IActionResult UploadPic(int id)
        {
            var viewModel = new TeacherPictureViewModel
            {
                ProfileImage = null,
                Teacher = null,

            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPic(int id, IFormFile file)
        {
            var viewModel = new TeacherPictureViewModel
            {
                Teacher = await _context.Teacher.FindAsync(id),
                ProfileImage = file,

            };

            string uniqueFileName = UploadedFile(viewModel.ProfileImage);
            viewModel.Teacher.ProfileImage = uniqueFileName;
            _context.Update(viewModel.Teacher);
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
