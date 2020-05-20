using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;

namespace University.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsApiController : ControllerBase
    {
        private readonly UniversityContext _context;

        public EnrollmentsApiController(UniversityContext context)
        {
            _context = context;
        }

        // GET: api/EnrollmentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollment(string courseTitle, string semestar, int? godina)
        {
            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();

            IQueryable<string> SemesterQuery = _context.Enrollment.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<int> GodinaQuery = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();


            if (!string.IsNullOrEmpty(courseTitle))
            {
                enrollments = enrollments.Where(m => m.Course.Title.Contains(courseTitle));
            }

            if (!string.IsNullOrEmpty(semestar))
            {
                enrollments = enrollments.Where(m => m.Semester.Contains(semestar));
            }
            if (godina != null)
            {
                enrollments = enrollments.Where(m => m.Year == godina);

            }
            return enrollments.ToList();
        }

        // GET: api/EnrollmentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return enrollment;
        }

        // PUT: api/EnrollmentsApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(int id, Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return BadRequest();
            }

            _context.Entry(enrollment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EnrollmentsApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Enrollment>> PostEnrollment(Enrollment enrollment)
        {
            _context.Enrollment.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnrollment", new { id = enrollment.Id }, enrollment);
        }

        // DELETE: api/EnrollmentsApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Enrollment>> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();

            return enrollment;
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
