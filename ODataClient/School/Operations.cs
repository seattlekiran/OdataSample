using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODataClient.ContosoSchoolReference.ODataService.Models.School;
using System.Data.Services.Client;

namespace ODataClient.School
{
    public static class Operations
    {
        static Uri baseUri = new Uri(string.Format("http://{0}:50231/", Environment.MachineName));

        #region Class

        public static void AddClass()
        {
            Container cntr = new Container(baseUri);

            Class algorithms = new Class();
            algorithms.Name = "Algorithms";

            cntr.AddObject("Classes", algorithms);
            cntr.SaveChanges();
        }

        public static void UpdateClass()
        {
            Container cntr = new Container(baseUri);

            Class cls = cntr.Classes.Where(clss => clss.Id == 1).Single();
            cls.Name = cls.Name + "-updated";

            cntr.UpdateObject(cls);
            cntr.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
        }

        public static void DeleteClass()
        {
            Container cntr = new Container(baseUri);

            Class cls = cntr.Classes.Where(clss => clss.Id == 1).Single();

            cntr.DeleteObject(cls);
            cntr.SaveChanges();
        }

        public static void GetAllClasses()
        {
            Container cntr = new Container(baseUri);

            foreach (Class cls in cntr.Classes)
            {
                Console.WriteLine(cls.Id + ": " + cls.Name);
            }
        }

        public static void GetAllStudentsInClass()
        {
            Container cntr = new Container(baseUri);

            var query = cntr.Classes.Where(clss => clss.Id == 2).SelectMany(clss => clss.Students);

            foreach (Student std in query)
            {
                Console.WriteLine(std.Name);
            }
        }

        #endregion

        #region Student

        public static void AddStudent()
        {
            Container cntr = new Container(baseUri);

            Student dan = new Student();
            dan.Name = "Dan";

            cntr.AddObject("Students", dan);
            cntr.SaveChanges();
        }

        public static void UpdateStudent()
        {
            Container cntr = new Container(baseUri);

            Student std = cntr.Students.Where(st => st.Id == 1).Single();
            std.Name = std.Name + "-updated";

            cntr.UpdateObject(std);
            cntr.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
        }

        public static void DeleteStudent()
        {
            Container cntr = new Container(baseUri);

            Student std = cntr.Students.Where(st => st.Id == 1).Single();

            cntr.DeleteObject(std);
            cntr.SaveChanges();
        }

        public static void GetAllStudents()
        {
            Container cntr = new Container(baseUri);

            foreach (Student std in cntr.Students)
            {
                Console.WriteLine(std.Name);
            }
        }

        public static void GetAllClassesOfStudent()
        {
            Container cntr = new Container(baseUri);

            var query = cntr.Students.Where(st => st.Id == 1).SelectMany(st => st.Classes);

            foreach (Class cls in query)
            {
                Console.WriteLine(cls.Name);
            }
        }

        public static void EnrollToClass()
        {
            Container cntr = new Container(baseUri);

            Class cls = cntr.Classes.Where(clss => clss.Id == 2).Single();
            Student std = cntr.Students.Where(st => st.Id == 1).Single();

            cntr.AddLink(std, "Classes", cls);
            cntr.SaveChanges();
        }

        #endregion

        #region Instructor

        public static void AddInstructor(string name)
        {
            Container cntr = new Container(baseUri);

            Instructor instr = new Instructor();
            instr.Name = name;

            cntr.AddObject("Instructors", instr);
            cntr.SaveChanges();
        }

        public static void UpdateInstructor()
        {
            Container cntr = new Container(baseUri);

            Instructor instr = cntr.Instructors.Where(inst => inst.Id == 1).Single();
            instr.Name = instr.Name + "-updated";

            cntr.UpdateObject(instr);
            cntr.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
        }

        public static void DeleteInstructor()
        {
            Container cntr = new Container(baseUri);

            Instructor instr = cntr.Instructors.Where(inst => inst.Id == 1).Single();

            cntr.DeleteObject(instr);
            cntr.SaveChanges();
        }

        public static void GetAllInstructors()
        {
            Container cntr = new Container(baseUri);

            foreach (Instructor instr in cntr.Instructors)
            {
                Console.WriteLine(instr.Name);
            }
        }

        public static void GetAllClassesOfInstructor()
        {
            Container cntr = new Container(baseUri);

            Instructor instr = cntr.Instructors.Where(inst => inst.Id == 1).Single();

            foreach (Class cls in instr.Classes)
            {
                Console.WriteLine(cls.Name);
            }
        }

        #endregion
    }
}
