using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataService.Models.Contoso
{
    public class Employee
    {
        public Employee()
        {
            this.Peers = new List<Employee>();
        }

        public int Id { get; set; }

        public string FullName { get; set; }

        //public string FirstName { get; set; }

        //public string MiddleName { get; set; }
        
        //public string LastName { get; set; }

        public List<Employee> Peers { get; set; }

        public Employee Manager { get; set; }
    }

    public class Manager : Employee
    {
        public Manager()
        {
            this.DirectReports = new List<Employee>();
        }

        public List<Employee> DirectReports { get; set; }
    }
}
