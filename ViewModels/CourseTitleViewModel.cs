using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class CourseTitleViewModel
    {
        public IList<Course> Courses { get; set; }
        public SelectList Programms { get; set; }
        public string CourseProgramm { get; set; }
        public SelectList Semesters { get; set; }

        public string CourseSemester { get; set; }


        public string searchString { get; set; }


        
        
    }
}
