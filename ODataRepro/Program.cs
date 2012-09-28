using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Formatter;
using System.Web.Http.SelfHost;
using System.Net.Http;
using System.Net;
using Microsoft.Data.OData;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.OData;

namespace ODataRepro
{
    public interface IVehicle
    {
        int Model { get; set; }
        string Name { get; set; }
    }

    public interface IVehicle1
    {
        int WheelCount { get; }
    }

    public class Vehicle
    {
        [Key]
        public int Model { get; set; }

        [Key]
        public string Name { get; set; }

        public virtual int WheelCount { get; set; }
    }

    public class Car : Vehicle
    {
        public override int WheelCount { get { return 4; } }

        public int SeatingCapacity { get; set; }
    }

    public class Motorcycle : Vehicle
    {
        public override int WheelCount { get { return 2; } }

        public bool CanDoAWheelie { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }

        public ICollection<Motorcycle> Motocycles { get; set; }

        public ICollection<SportBike> SportBikes { get; set; }
    }

    public class SportBike : Motorcycle
    {
        public long TopSpeed { get; set; }
    }

    public class MotorcyclesController : ApiController
    {
        public IEnumerable<Motorcycle> Get()
        {
            return new Motorcycle[] {
                new Motorcycle {
                    Name = "Test"
                }
            };
        }

        public void Put(Delta<Vehicle> motocycle)
        {

        }
    }

    class Program
    {
        static IEdmModel GetEdmModel()
        {
            ODataModelBuilder builder = new ODataModelBuilder();

            //builder
            //    .Entity<IVehicle>()

            //builder
            //    .Entity<IVehicle1>()
            
            builder
                .Entity<Vehicle>()
                .Abstract()
                .HasKey(v => v.Name)
                .HasKey(v => v.Model)
                .Property(v => v.WheelCount);

            var motocycle = builder
                .Entity<Motorcycle>()
                .DerivesFrom<Vehicle>();
            motocycle.Property(m => m.CanDoAWheelie);
            motocycle.HasMany(m => m.Vehicles);
            motocycle.HasMany(m => m.Motocycles);
            motocycle.HasMany(m => m.SportBikes);

            builder
                .Entity<Car>()
                .DerivesFrom<Vehicle>()
                .Property(c => c.SeatingCapacity);

            builder
                .Entity<SportBike>()
                .DerivesFrom<Motorcycle>()
                .Property(b => b.TopSpeed);

            var vechicles = builder.EntitySet<Vehicle>("vehicles");
            var motocycles = builder.EntitySet<Motorcycle>("motorcycles");
            var cars = builder.EntitySet<Car>("cars");
            var sportBikes = builder.EntitySet<SportBike>("sportBikes");


            return builder.GetEdmModel();
        }

        static readonly Uri _baseAddress = new Uri("http://localhost:50232/");

        static void Main(string[] args)
        {
            Console.WriteLine(typeof(int).BaseType.BaseType);

            HttpSelfHostServer server = null;

            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration(_baseAddress);
                configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;

                // Generate a model
                IEdmModel model = GetEdmModel();

                // Create the OData formatter and give it the model
                ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter(model);
                configuration.Formatters.Clear();
                configuration.SetODataFormatter(odataFormatter);

                // Metadata routes to support $metadata and code generation in the WCF Data Service client.
                configuration.Routes.MapHttpRoute(ODataRouteNames.Metadata, "$metadata", new { Controller = "ODataMetadata", Action = "GetMetadata" });
                configuration.Routes.MapHttpRoute(ODataRouteNames.ServiceDocument, "", new { Controller = "ODataMetadata", Action = "GetServiceDocument" });

                // Relationship routes (notice the parameters is {type}Id not id, this avoids colliding with GetById(id)).
                configuration.Routes.MapHttpRoute(ODataRouteNames.PropertyNavigation, "{controller}({parentId})/{navigationProperty}");

                // Route for manipulating links.
                //configuration.Routes.MapHttpRoute(ODataRouteNames.Link, "{controller}({id})/$links/Products");
                configuration.Routes.MapHttpRoute(ODataRouteNames.Link, "{controller}({id})/$links/{navigationProperty}");

                // Routes for urls both producing and handling urls like ~/Product(1), ~/Products() and ~/Products
                configuration.Routes.MapHttpRoute(ODataRouteNames.GetById, "{controller}({id})");
                configuration.Routes.MapHttpRoute(ODataRouteNames.DefaultWithParentheses, "{controller}()");
                configuration.Routes.MapHttpRoute(ODataRouteNames.Default, "{controller}");

                // Create server
                server = new HttpSelfHostServer(configuration);

                // Start listening
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + _baseAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server: {0}", e.GetBaseException().Message);
            }
            finally
            {
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();

                if (server != null)
                {
                    // Stop listening
                    server.CloseAsync().Wait();
                }
            }
        }
    }
}
