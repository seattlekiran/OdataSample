using ODataService.Models.Contoso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;
namespace ODataService.Controllers.Contoso
{
    public class EmployeesController : ApiController
    {
        public Employee Get()
        {
            Manager emp = new Manager();
            emp.FullName = "Jane";
            emp.Id = 1;

            Employee emp1 = new Employee();
            emp1.Id = 2;
            emp1.FullName = "kiran";
            emp1.Manager = emp;


            Employee emp2 = new Employee();
            emp2.Id = 3;
            emp2.FullName = "hongye";
            emp2.Manager = emp;

            emp.DirectReports.Add(emp1);
            emp.DirectReports.Add(emp2);

            return emp;
        }

        public IEnumerable<Employee> GetDirectReports(int parentId)
        {
            Manager emp = new Manager();
            emp.FullName = "Jane";
            emp.Id = 1;

            Employee emp1 = new Employee();
            emp1.Id = 2;
            emp1.FullName = "kiran";
            emp1.Manager = emp;


            Employee emp2 = new Employee();
            emp2.Id = 3;
            emp2.FullName = "hongye";
            emp2.Manager = emp;

            emp.DirectReports.Add(emp1);
            emp.DirectReports.Add(emp2);

            return emp.DirectReports;
        }

        public Employee Post(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState));
            }
        

            return emp;
        }
    }
}
