using ContosoSchool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ODataService.Controllers
{
    public class StudentsController : ApiController
    {
        ContosoSchoolContext dbCtxt = new ContosoSchoolContext();

        public HttpResponseMessage PostStudent(Student stud)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState));
            }

            dbCtxt.Students.Add(stud);
            int records = dbCtxt.SaveChanges();

            var response = Request.CreateResponse(HttpStatusCode.Created, stud);
            response.Headers.Location = new Uri(Url.Link(ODataRouteNames.GetById, new { Controller = "Students", Id = stud.Id }));
            return response;
        }

        public Student GetStudent(int id)
        {
            return dbCtxt.Students.Where(stud => stud.Id == id).Single();
        }

        public void DeleteStudent(int id)
        {
            var stud = dbCtxt.Students.Where(i => i.Id == id).Single();

            dbCtxt.Students.Remove(stud);
            dbCtxt.SaveChanges();
        }

        public void PutStudent(int id, Student stud)
        {
            stud.Id = id;
            dbCtxt.Students.Attach(stud);
            dbCtxt.Entry(stud).State = System.Data.EntityState.Modified;
            dbCtxt.SaveChanges();
        }
    }
}
