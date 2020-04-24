using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Data;
using University.Models;


namespace University.Models
{
    public class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new UniversityContext(serviceProvider.GetRequiredService<DbContextOptions<UniversityContext>>()))
            {
                if (context.Course.Any() || context.Student.Any() || context.Teacher.Any())
                {
                    return;   // DB has been seeded
                }

                context.Student.AddRange(
                    new Student {  /*Id = 1, */FirstName = "Rob", LastName = "Reiner", EnrollmentDate = DateTime.Parse("1947-3-6"), AcquiredCredits=60,CurrentSemester=6,EducationLevel="undergrad" },
                    new Student {  /*Id = 2, */FirstName = "Andrej", LastName = "Popov", EnrollmentDate = DateTime.Parse("2017-9-1"), AcquiredCredits = 60, CurrentSemester = 6, EducationLevel = "undergrad" },
                    new Student {  /*Id = 3, */FirstName = "Filip", LastName = "Pancevski", EnrollmentDate = DateTime.Parse("2017-9-1"), AcquiredCredits = 60, CurrentSemester = 6, EducationLevel = "undergrad" },
                    new Student {  /*Id = 4, */FirstName = "David", LastName = "Petrusevski", EnrollmentDate = DateTime.Parse("2017-9-1"), AcquiredCredits = 60, CurrentSemester = 6, EducationLevel = "undergrad" },
                    new Student {  /*Id = 5, */FirstName = "Angela", LastName = "Tasik", EnrollmentDate = DateTime.Parse("2017-9-1"), AcquiredCredits = 60, CurrentSemester = 6, EducationLevel = "undergrad" },
                    new Student {  /*Id = 6, */FirstName = "Sara", LastName = "Stojanovska", EnrollmentDate = DateTime.Parse("2017-9-1"), AcquiredCredits = 60, CurrentSemester = 6, EducationLevel = "undergrad" },
                    new Student {  /*Id = 7, */FirstName = "Rob", LastName = "Reiner", EnrollmentDate = DateTime.Parse("2017-9-1"), AcquiredCredits = 60, CurrentSemester = 6, EducationLevel = "undergrad" }
                );
                context.SaveChanges();

                context.Teacher.AddRange(
                    new Teacher {/*Id = 1, */FirstName = "Rob", LastName = "Reiner", Degree = "neznam",AcademicRank="neznam",OfficeNumber="neznam",HireDate= DateTime.Parse("2017-9-1")},
                    new Teacher {/*Id = 2, */FirstName = "Rop", LastName = "Reiner", Degree = "neznam", AcademicRank = "neznam", OfficeNumber = "neznam", HireDate = DateTime.Parse("2017-9-1") },
                    new Teacher {/*Id = 3, */FirstName = "Ros", LastName = "Reiner", Degree = "neznam", AcademicRank = "neznam", OfficeNumber = "neznam", HireDate = DateTime.Parse("2017-9-1") },
                    new Teacher {/*Id = 4, */FirstName = "Roq", LastName = "Reiner", Degree = "neznam", AcademicRank = "neznam", OfficeNumber = "neznam", HireDate = DateTime.Parse("2017-9-1") }
                    );
                context.SaveChanges();


                context.Course.AddRange(
                    new Course {/*Id = 1, */Title="RSWA", Credits = 6, Semester=5, Programme="blabla", EducationLevel="nz", FirstTeacherId=context.Teacher.Single(d=>d.FirstName=="Rob" && d.LastName=="Reiner").Id,SecondTeacherId=context.Teacher.Single(d=>d.FirstName=="Roq"&&d.LastName=="Reiner").Id},
                    new Course {/*Id = 2, */Title = "MSWA", Credits = 6, Semester = 5, Programme = "blabla", EducationLevel = "nz", FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Rob" && d.LastName == "Reiner").Id, SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Roq" && d.LastName == "Reiner").Id },
                    new Course {/*Id = 3, */Title = "Opticki Mrezi", Credits = 6, Semester = 5, Programme = "blabla", EducationLevel = "nz", FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Rob" && d.LastName == "Reiner").Id, SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Roq" && d.LastName == "Reiner").Id },
                    new Course {/*Id = 4, */Title = "Matematika1", Credits = 6, Semester = 5, Programme = "blabla", EducationLevel = "nz", FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Rob" && d.LastName == "Reiner").Id, SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Roq" && d.LastName == "Reiner").Id }
                    );
                context.SaveChanges();

                context.Enrollment.AddRange(
                  new Enrollment { CourseId = 1, StudentId = 1 },
                  new Enrollment{ CourseId = 2, StudentId = 4 },
                  new Enrollment { CourseId = 3, StudentId = 1 },
                  new Enrollment { CourseId = 4, StudentId = 6},
                  new Enrollment { CourseId = 4, StudentId = 2 },
                  new Enrollment { CourseId = 4, StudentId = 7 },
                  new Enrollment { CourseId = 4, StudentId = 3 },
                  new Enrollment { CourseId = 4, StudentId = 3 }

              );
                context.SaveChanges();


            }
        }
    }
}
