using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

namespace ODataService.Controllers
{
    public class CustomersController : ApiController
    {
        
    }

    public class IndividualsController : CustomersController
    {
        public Customer ChangeName(int parentId, ODataActionParameters odataParams)
        {
            IndividualCustomer ic = new IndividualCustomer();
            ic.Id = 1;
            ic.Name = "kiran";
            ic.SSN = "111111111";

            return ic;
        }
    }

    public class BusinessesController : CustomersController
    {
    }
}
