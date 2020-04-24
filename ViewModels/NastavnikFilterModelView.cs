using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.ViewModels
{
    public class NastavnikFilterModelView
    {
        public IList<Teacher> Teachers { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public SelectList Obrazovanie { get; set; }
        public string obrazovanie { get; set; }

        public SelectList AkademskiRank { get; set; }
        public string akademskiRank { get; set; }

    }
}
