using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODataClient.ContosoSchoolReference.ContosoSchool;

namespace ODataClient
{
    public static class ContosoSchoolTests
    {
        public static void AddClass()
        {
            Container cntr = new Container(new Uri("http://kclaptop:50231/"));

            Class algorithms = new Class();
            algorithms.Name = "Algorithms";

            cntr.AddObject("Classes", algorithms);
            cntr.SaveChanges();
        }
    }
}
