using Microsoft.Data.Edm;
using ODataService.Models;
using ODataService.Models.Contoso;
using ODataService.Models.School;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Builder.Conventions;
using System.Web.Http.OData.Formatter;
using System.Web.Http.SelfHost;

namespace ODataService
{
    /// <summary>
    /// Runs a sample OData Service that exposes, Products, ProductFamilies and Suppliers.
    /// </summary>
    class Program
    {
        static readonly Uri _baseAddress = new Uri(string.Format("http://{0}:50231/", Environment.MachineName));

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;

            try
            {
                //DbMigrator migrator = new DbMigrator(new Migrations.Configuration());
                //migrator.Update(null);
                //return;

                // Set up server configuration
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration(_baseAddress);
                configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

                //TraceConfig.Register(configuration);

                // Register an Action selector that can include template parameters in the name
                configuration.Services.Replace(typeof(IHttpActionSelector), new ODataActionSelector());

                // Generate a model
                IEdmModel model = GetEdmModel();
                //configuration.SetEdmModel(model);

                // Create the OData formatter and give it the model
                ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter(model);
                configuration.SetODataFormatter(odataFormatter);

                // Metadata routes to support $metadata and code generation in the WCF Data Service client.
                configuration.Routes.MapHttpRoute(ODataRouteNames.Metadata, "$metadata", new { Controller = "ODataMetadata", Action = "GetMetadata" });
                configuration.Routes.MapHttpRoute(ODataRouteNames.ServiceDocument, "", new { Controller = "ODataMetadata", Action = "GetServiceDocument" });

                // Relationship routes (notice the parameters is {type}Id not id, this avoids colliding with GetById(id)).
                configuration.Routes.MapHttpRoute("ExtendSupportDateOnProductsColl", "Products/ExtendSupportDates", new { controller = "Products", action = "ExtendSupportDates" });
                configuration.Routes.MapHttpRoute(ODataRouteNames.InvokeBoundAction, "Products({boundId})/ExtendSupportDate", new { controller = "Products", odataAction = "ExtendSupportDate" });
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

        /// <summary>
        /// Get the EdmModel.
        /// </summary>
        /// <returns></returns>
        static IEdmModel GetEdmModel()
        {
            // build the model by convention
            return GetImplicitEdmModel();

            // or build the model by hand
            //return GetExplicitEdmModel();

            //return GetImplicit_InheritanceModel();

            //return GetContosoModel();
        }

        /// <summary>
        /// Generates a model explicitly.
        /// </summary>
        /// <returns></returns>
        static IEdmModel GetExplicitEdmModel()
        {
            ODataModelBuilder modelBuilder = new ODataModelBuilder();

            //var type1s = modelBuilder.EntitySet<Type1>("Type1s");
            //type1s.HasEditLink(entityContext => entityContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "Type1s", id = entityContext.EntityInstance.Id }));

            //var type2s = modelBuilder.EntitySet<Type2>("Type2s");
            //type2s.HasEditLink(entityContext => entityContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "Type2s", id = entityContext.EntityInstance.Id }));

            //var type1 = type1s.EntityType;
            //type1.HasKey(p => p.Id);
            //type1.Property(p => p.Name);

            //var type2 = type2s.EntityType;
            //type2.HasKey(p => p.Id);
            //type2.Property(p => p.Name);

            //type1s.HasRequiredBinding(p => p.Type2Prop, type2s);
            //type2s.HasRequiredBinding(p => p.Type1Prop, type1s);

            //return modelBuilder.GetEdmModel();

            var products = modelBuilder.EntitySet<Product>("Products");
            //products.HasEditLink(entityContext => entityContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "Products", id = entityContext.EntityInstance.ID }));
            products.HasEditLink(instanceContext =>
                {
                    return instanceContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "Products", id = instanceContext.EntityInstance.ID });
                });

            var suppliers = modelBuilder.EntitySet<Supplier>("Suppliers");
            suppliers.HasEditLink(entityContext => entityContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "Suppliers", id = entityContext.EntityInstance.ID }));

            var families = modelBuilder.EntitySet<ProductFamily>("ProductFamilies");
            families.HasEditLink(entityContext => entityContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "ProductFamilies", id = entityContext.EntityInstance.ID }));

            var product = products.EntityType;

            product.HasKey(p => p.ID);
            product.Property(p => p.Name);
            product.Property(p => p.ReleaseDate);
            product.Property(p => p.SupportedUntil);

            var address = modelBuilder.ComplexType<Address>();
            address.Property(a => a.City);
            address.Property(a => a.Country);
            address.Property(a => a.State);
            address.Property(a => a.Street);
            address.Property(a => a.ZipCode);

            var supplier = suppliers.EntityType;
            supplier.HasKey(s => s.ID);
            supplier.Property(s => s.Name);
            supplier.CollectionProperty(s => s.Addresses);
            supplier.CollectionProperty(s => s.Tags);
            supplier.Property(s => s.Country);

            var productFamily = families.EntityType;
            productFamily.HasKey(pf => pf.ID);
            productFamily.Property(pf => pf.Name);
            productFamily.Property(pf => pf.Description);

            // Create relationships and bindings in one go
            products.HasRequiredBinding(p => p.Family, families);
            families.HasManyBinding(pf => pf.Products, products);
            families.HasOptionalBinding(pf => pf.Supplier, suppliers);
            suppliers.HasManyBinding(s => s.ProductFamilies, families);

            // Create navigation Link builders
            products.HasNavigationPropertiesLink(
                product.NavigationProperties,
                (entityContext, navigationProperty) => new Uri(entityContext.UrlHelper.Link(ODataRouteNames.PropertyNavigation, new { Controller = "Products", parentId = entityContext.EntityInstance.ID, NavigationProperty = navigationProperty.Name })));
            families.HasNavigationPropertiesLink(
                productFamily.NavigationProperties,
                (entityContext, navigationProperty) => new Uri(entityContext.UrlHelper.Link(ODataRouteNames.PropertyNavigation, new { Controller = "Categories", parentId = entityContext.EntityInstance.ID, NavigationProperty = navigationProperty.Name })));
            suppliers.HasNavigationPropertiesLink(
                supplier.NavigationProperties,
                (entityContext, navigationProperty) => new Uri(entityContext.UrlHelper.Link(ODataRouteNames.PropertyNavigation, new { Controller = "Suppliers", parentId = entityContext.EntityInstance.ID, NavigationProperty = navigationProperty.Name })));

            return modelBuilder.GetEdmModel();
        }

        /// <summary>
        /// Generates a model from a few seeds (i.e. the names and types of the entity sets)
        /// by applying conventions.
        /// </summary>
        /// <returns>An implicitly configured model</returns>    
        static IEdmModel GetImplicitEdmModel()
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            var products = modelBuilder.EntitySet<Product>("Products");
            var productFamilies = modelBuilder.EntitySet<ProductFamily>("ProductFamilies");
            var suppliers = modelBuilder.EntitySet<Supplier>("Suppliers");
            
            var config = products.EntityType.Collection.Action("ExtendSupportDates");
            config.Parameter<DateTime>("newDate");
            config.Parameter<int[]>("ids");
            config.ReturnsCollectionFromEntitySet<Product>("Products");

            var config2 = products.EntityType.Action("ExtendSupportDate");
            config2.Parameter<DateTime>("newDate");
            config2.ReturnsFromEntitySet<Product>("Products");
            config2.HasActionLink(eic =>
                {
                    //Do not advertise 'ExtendSupportDate' for discontinued products
                    if (eic.EntityType.Name == "DiscontinuedProduct")
                        return null;

                    //Advertise for the rest of them
                    Product pd = (Product)eic.EntityInstance;

                    return new Uri(eic.UrlHelper.Link(ODataRouteNames.InvokeBoundAction, new
                    {
                        controller = eic.EntitySet.Name,
                        boundId = pd.ID,
                        odataAction = "ExtendSupportDate"
                    }));
                });

            return modelBuilder.GetEdmModel();
        }

        static IEdmModel GetImplicit_InheritanceModel()
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            var vehicles = modelBuilder.EntitySet<Vehicle>("Vehicles");
            var cars = modelBuilder.EntitySet<Car>("Cars");
            var motorcycles = modelBuilder.EntitySet<Motorcycle>("Motorcycles");

            //var config = cars.EntityType.Action("Drive");
            //config.ReturnsFromEntitySet<Car>("Cars");

            //var config1 = motorcycles.EntityType.Action("Drive");
            //config1.ReturnsFromEntitySet<Motorcycle>("Motorcycles");

            return modelBuilder.GetEdmModel();
        }

        static IEdmModel GetContosoModel()
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            var employees = modelBuilder.EntitySet<Employee>("Employees");

            return modelBuilder.GetEdmModel();
        }
    }

    public class DiscontinuedProduct : Product
    {
    }
}
