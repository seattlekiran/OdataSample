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
    public class InstructorsController : ApiController
    {
        ContosoSchoolContext dbCtxt = new ContosoSchoolContext();

        public HttpResponseMessage PostInstructor(Instructor instr)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState));
            }

            dbCtxt.Instructors.Add(instr);
            int records = dbCtxt.SaveChanges();

            var response = Request.CreateResponse(HttpStatusCode.Created, instr);
            response.Headers.Location = new Uri(Url.Link(ODataRouteNames.GetById, new { Controller = "Instructors", Id = instr.Id }));
            return response;
        }

        public Instructor GetInstructor(int id)
        {
            return dbCtxt.Instructors.Where(instr => instr.Id == id).Single();
        }

        public void DeleteInstructor(int id)
        {
            var instr = dbCtxt.Instructors.Where(i => i.Id == id).Single();

            dbCtxt.Instructors.Remove(instr);
            dbCtxt.SaveChanges();
        }

        public void PutInstructor(int id, Instructor instr)
        {
            instr.Id = id;
            dbCtxt.Instructors.Attach(instr);
            dbCtxt.Entry(instr).State = System.Data.EntityState.Modified;
            dbCtxt.SaveChanges();
        }
    }
}
