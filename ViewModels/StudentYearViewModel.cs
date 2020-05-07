using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class StudentYearViewModel
    {
        public IList<Student> Students { get; set; }
        public IList<Enrollment> Enrollments { get; set; }
        public IList<Course> Courses { get; set; }

       public  int Year { get; set; }
    }
}
