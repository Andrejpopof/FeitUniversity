using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class StudentCourseEdit
    {
    public Student Student { get; set; }
    public IEnumerable<int> SelectedCourses { get; set; }
    public IEnumerable<SelectListItem> CourseList { get; set; }
}
}
