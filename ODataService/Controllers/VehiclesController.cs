using ODataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

namespace ODataService.Controllers
{
    //public class VehiclesController : ApiController
    //{
    //    public IEnumerable<Vehicle> Get()
    //    {
    //        List<Vehicle> vehicles = new List<Vehicle>();
    //        vehicles.Add(new Motorcycle()
    //        {
    //            Id = 1,
    //            Model = "Ducati",
    //            Name = "Z50",
    //            CanDoAWheelie = true,
    //        });

    //        return vehicles;
    //    }
    //}

    public class MotorcyclesController : ApiController
    {
        //public IEnumerable<Motorcycle> Get()
        //{
        //    List<Motorcycle> vehicles = new List<Motorcycle>();
        //    vehicles.Add(new Motorcycle()
        //    {
        //        Id = 1,
        //        Model = "Ducati",
        //        Name = "Z50",
        //        CanDoAWheelie = true,
        //    });

        //    return vehicles;
        //}

        [HttpPost]
        public Motorcycle SetWheelCount(int boundId, ODataActionParameters actionParams)
        {
            return new Motorcycle()
            {
                Id = 1,
                Model = "Ducati",
                Name = "Z50",
                CanDoAWheelie = true,
            };
        }
    }

    public class CarsController : ApiController
    {
        //public IEnumerable<Car> Get()
        //{
        //    List<Car> vehicles = new List<Car>();
        //    vehicles.Add(new Car()
        //    {
        //        Id = 1,
        //        Model = "Ducati",
        //        Name = "Z50"
        //    });

        //    return vehicles;
        //}

        [HttpPost]
        public Car SetWheelCount(int boundId, ODataActionParameters actionParams)
        {
            return new Car()
            {
                Id = 1,
                Model = "Ducati",
                Name = "Z50",
            };
        }
    }
}
