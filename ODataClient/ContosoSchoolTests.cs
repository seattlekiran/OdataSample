using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODataClient.ContosoSchoolReference.ODataService.Models.School;

namespace ODataClient
{
    public static class ContosoSchoolTests
    {
        static Uri baseUri = new Uri(string.Format("http://{0}:50231/", Environment.MachineName));

        public static void AddClass()
        {
            Container cntr = new Container(baseUri);

            Class algorithms = new Class();
            algorithms.Name = "Algorithms";

            cntr.AddObject("Classes", algorithms);
            cntr.SaveChanges();
        }
    }
}
