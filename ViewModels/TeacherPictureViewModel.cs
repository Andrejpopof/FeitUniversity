using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class TeacherPictureViewModel
    {
        public Teacher Teacher { get; set;}

        public IFormFile ProfileImage { get; set; }
    }
}
