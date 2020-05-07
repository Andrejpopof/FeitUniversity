using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class EnrollmentsSearchViewModelcs
    {
        public IList<Enrollment> Enrollments { get; set; }
        public int godina { get; set; }

        public SelectList Semester { get; set; }
        public string semestar { get; set; }

        public IList<Course> Courses { get; set; }
        public string courseTitle { get; set; }
    }
}
