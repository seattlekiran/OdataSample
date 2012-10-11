using ODataService.Models.Contoso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ODataService.Controllers.Contoso
{
    public class EmployeesController : ApiController
    {
        public Employee GetEntityById(int id)
        {
            Employee emp = new Employee();
            emp.FullName = "Kiran";
            emp.Id = 1;
            emp.Manager = new Employee();
            emp.Manager.FullName = "Jane";
            emp.Peers.Add(new Employee() { Id = 2, FullName = "Bhavesh" });
            
            return emp;
        }
    }
}
