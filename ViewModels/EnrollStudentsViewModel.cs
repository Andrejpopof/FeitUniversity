using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class EnrollStudentsViewModel
    {
        public int CourseId { get; set; }
        public IEnumerable<int> SelectedStudents { get; set; }

        public IEnumerable<SelectListItem> StudentList { get; set; }

        
        public int Year { get; set; }

        public string Semester { get; set; }

        public Enrollment Enrollment { get; set; }

        public SelectList CoursesList { get; set; }
    }
}
