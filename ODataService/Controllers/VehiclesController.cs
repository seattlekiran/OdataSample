using ODataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.OData;

namespace ODataService.Controllers
{
    public class VehiclesController : ApiController
    {
        public IEnumerable<Vehicle> Get()
        {
            List<Vehicle> vehicles = new List<Vehicle>();
            //vehicles.Add(new Vehicle()
            //{
            //    Id = 1,
            //    Model = "Toyota",
            //    Name = "Tacoma",
            //    WheelCount = 3
            //});
            vehicles.Add(new Car()
            {
                Id = 2,
                Model = "BMW",
                Name = "Mini",
            });

            //vehicles.Add(new Motorcycle()
            //{
            //    Id = 3,
            //    Model = "Ducati",
            //    Name = "Z50",
            //    WheelCount = 3
            //});


            return vehicles;
        }

        public virtual Vehicle Drive(int boundId, ODataActionParameters actionParams)
        {
            return new Motorcycle()
            {
                Id = 1,
                Model = "Toyota",
                Name = "Tacoma"
            };
        }

        public Vehicle Post(Vehicle v)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState));
            }

            return v;
        }
    }

    //public class MotorcyclesController : VehiclesController
    //{
    //    public IEnumerable<Motorcycle> Get()
    //    {
    //        List<Motorcycle> vehicles = new List<Motorcycle>();
    //        vehicles.Add(new SportBike()
    //        {
    //            Id = 1,
    //            Model = "Ducati",
    //            Name = "Z50",
    //             CanDoAWheelie = true,
    //              TopSpeed = 350
    //        });

    //        return vehicles;
    //    }

    //    public override Vehicle Drive(int boundId, ODataActionParameters actionParams)
    //    {
    //        return new Motorcycle()
    //        {
    //            Id = 1,
    //            Model = "Ducati",
    //            Name = "Z50",
    //            CanDoAWheelie = true,
    //        };
    //    }
    //}

    //public class CarsController : VehiclesController
    //{
    //    public IEnumerable<Car> Get()
    //    {
    //        List<Car> cars = new List<Car>();
    //        cars.Add(new Car()
    //        {
    //            Id = 1,
    //            Model = "BMW",
    //            Name = "Mini",
    //             SeatingCapacity = 2
    //        });

    //        return cars;
    //    }

    //    public override Vehicle Drive(int boundId, ODataActionParameters actionParams)
    //    {
    //        return new Car()
    //        {
    //            Id = 1,
    //            Model = "BMW",
    //            Name = "Mini",
    //            SeatingCapacity = 2
    //        };
    //    }
    //}
}
