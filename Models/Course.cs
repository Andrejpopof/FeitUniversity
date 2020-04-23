using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace University.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }



        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public int Credits { get; set; }

        [Required]
        public int Semester { get; set; }

        [StringLength(100)]
        public string Programme { get; set; }

        [StringLength(25)]
        public string EducationLevel { get; set; }

        public int? FirstTeacherId { get; set; }
        public int? SecondTeacherId { get; set; }

        [Display(Name = "First Teacher")]
        public Teacher FirstTeacher { get; set; }
        
        [Display(Name ="Second Teacher")]
       public Teacher SecondTeacher { get; set; }

        public ICollection<Enrollment> Students { get; set; }
    }
}
