using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace University.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        [StringLength(10)]
        public string Semester { get; set; }
        public int Year { get; set; }
        public int Grade { get; set; }

        [StringLength(255)]
        public string SeminalUrl { get; set; }

        [StringLength(255)]
        public string ProjectUrl { get; set; }
        public int ExamPoints { get; set; }
        public int SeminalPoints { get; set; }
        public int ProjectPoints { get; set; }
        public int AdditionalPoints { get; set; }

        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }
    }
}
