using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using University.Models;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions<UniversityContext> options)
            : base(options) { }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.FirstTeacher)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.FirstTeacherId);
            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.SecondTeacher)
                .WithMany(p => p.Courses1)
                .HasForeignKey(p => p.SecondTeacherId);
            builder.Entity<Enrollment>()
                .HasOne<Course>(p => p.Course)
                .WithMany(p => p.Students)
                .HasForeignKey(p => p.CourseId);
            builder.Entity<Enrollment>()
                .HasOne<Student>(p => p.Student)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.StudentId);
 }



        public DbSet<University.Models.Student> Student { get; set; }



        public DbSet<University.Models.Course> Course { get; set; }



        public DbSet<University.Models.Teacher> Teacher { get; set; }

        public DbSet<Enrollment> Enrollment { get; set; }

    }

}
