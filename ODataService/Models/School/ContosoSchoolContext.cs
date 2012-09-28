using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataService.Models.School
{
    public class ContosoSchoolContext : DbContext
    {
        public ContosoSchoolContext()
            : base("ContosoSchoolDb")
        {
        }

        public DbSet<Class> Classes { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Instructor> Instructors { get; set; }
    }
}
