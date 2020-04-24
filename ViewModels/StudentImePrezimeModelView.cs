using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class StudentImePrezimeModelView
    {
        public IList<Student> Students { get; set; }
        public string firstName { get; set; }
        public string LastName { get; set; }



    }
}
