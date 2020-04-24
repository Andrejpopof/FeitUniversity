using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace University.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }


        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        public int AcquiredCredits { get; set; }

        public int CurrentSemester { get; set; }

        [StringLength(25)]
        public string EducationLevel { get; set; }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        public ICollection<Enrollment> Courses { get; set; }
    }
}
