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
    public class ClassesController : ApiController
    {
        ContosoSchoolContext dbCtxt = new ContosoSchoolContext();

        public HttpResponseMessage PostClass(Class clss)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState));
            }

            dbCtxt.Classes.Add(clss);
            int records = dbCtxt.SaveChanges();

            var response = Request.CreateResponse(HttpStatusCode.Created, clss);
            response.Headers.Location = new Uri(Url.Link(ODataRouteNames.GetById, new { Controller = "Classes", Id = clss.Id }));
            return response;
        }

        public Class GetClass(int id)
        {
            return dbCtxt.Classes.Where(clss => clss.Id == id).Single();
        }

        public void DeleteClass(int id)
        {
            var clss = dbCtxt.Classes.Where(c => c.Id == id).Single();

            dbCtxt.Classes.Remove(clss);
            dbCtxt.SaveChanges();
        }

        public void PutClass(int id, Class clss)
        {
            clss.Id = id;
            dbCtxt.Classes.Attach(clss);
            dbCtxt.Entry(clss).State = System.Data.EntityState.Modified;
            dbCtxt.SaveChanges();
        }
    }
}
