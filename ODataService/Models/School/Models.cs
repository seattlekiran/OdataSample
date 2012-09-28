using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataService.Models.School
{
    public class Class
    {
        public Class()
        {
            this.Students = new List<Student>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Instructor Instructor { get; set; }
        public List<Student> Students { get; set; }
    }

    public class Student
    {
        public Student()
        {
            this.Classes = new List<Class>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Class> Classes { get; set; }
    }

    public class Instructor
    {
        public Instructor()
        {
            this.Classes = new List<Class>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Class> Classes { get; set; }
    }
}
