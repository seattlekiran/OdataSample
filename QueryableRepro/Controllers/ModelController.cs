using QueryableRepro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QueryableRepro.Controllers
{
    public class ModelController : ApiController
    {
        // GET api/model
        [Queryable]
        public IEnumerable<Model> Get()
        {
            return new Model[] { 
                new Model() {
                    Enum1 = Enum1.One,
                    Enum10 = Enum1.One,
                    Enum2 = Enum2.One,
                    Enum3 = Enum3.One
                },
                new Model() {
                    Enum1 = Enum1.One,
                    Enum10 = Enum1.Two,
                    Enum2 = Enum2.Two,
                    Enum3 = Enum3.Two | Enum3.One
                },
                new Model() {
                    Enum1 = Enum1.One,
                    Enum10 = Enum1.Three,
                    Enum2 = Enum2.Three,
                    Enum3 = Enum3.Three | Enum3.Two | Enum3.One
                },
                new Model() {
                    Enum1 = Enum1.One,
                    //Enum10 = Enum1.Two,
                    Enum2 = Enum2.One,
                    Enum3 = Enum3.One
                }
            };
        }

        // GET api/model/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/model
        public void Post([FromBody]string value)
        {
        }

        // PUT api/model/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/model/5
        public void Delete(int id)
        {
        }
    }
}
