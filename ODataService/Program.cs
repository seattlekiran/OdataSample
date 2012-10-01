﻿using Microsoft.Data.Edm;
using ODataService.Models;
using ODataService.Models.School;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
                //DbMigrator migrator = new DbMigrator(new ContosoSchoolMigrationConfiguration());
                //migrator.Update(null);

                // Set up server configuration
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration(_baseAddress);
                configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;

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
        }

        /// <summary>
        /// Generates a model explicitly.
        /// </summary>
        /// <returns></returns>
        static IEdmModel GetExplicitEdmModel()
        {
            ODataModelBuilder modelBuilder = new ODataModelBuilder();

            var products = modelBuilder.EntitySet<Product>("Products");
            products.HasEditLink(entityContext => entityContext.UrlHelper.Link(ODataRouteNames.GetById, new { controller = "Products", id = entityContext.EntityInstance.ID }));

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
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            //modelBuilder.ComplexType<Address>();
            //modelBuilder.EntitySet<Product>("Products");
            //modelBuilder.EntitySet<ProductFamily>("ProductFamilies");
            //modelBuilder.EntitySet<Supplier>("Suppliers");

            var classEType = modelBuilder.EntitySet<Class>("Classes").EntityType;
            var studentEType = modelBuilder.EntitySet<Student>("Students").EntityType;
            var instructorEType = modelBuilder.EntitySet<Instructor>("Instructors").EntityType;

            var enrollClass = studentEType.Action("EnrollClass");
            enrollClass.Parameter<int>("classId");

            ActionConfiguration test = new ActionConfiguration(modelBuilder, "UnboundedActionExample");
            test.Parameter<ClassType>("classType");
            test.CollectionParameter<ClassType>("collOfClassType");

            test.Parameter<string>("stringParam");
            test.CollectionParameter<string>("collOfString");

            test.Parameter<Nullable<Guid>>("guidParam");
            test.CollectionParameter<Guid>("collOfGuid");

            test.Parameter<Nullable<char>>("charParam");
            test.CollectionParameter<char>("collOfChar");

            test.Parameter<Nullable<bool>>("boolParam");
            test.CollectionParameter<bool>("arrayOfBoolParam");
            
            test.Parameter<Nullable<byte>>("byteParam");
            test.CollectionParameter<byte>("listOfByteParam");

            test.Parameter<Nullable<Int16>>("int16Param");
            test.CollectionParameter<Int16>("listOfInt16Param");
            
            test.Parameter<Int32>("int32Param");
            test.CollectionParameter<Int32>("listOfInt32Param");

            test.Parameter<Int64>("int64Param");
            test.Parameter<UInt16>("uint16Param");
            test.Parameter<UInt32>("uint32Param");
            test.Parameter<UInt64>("uint64Param");

            test.Parameter<Address>("addressParam");
            test.CollectionParameter<Address>("listOfAddressParam");
            test.CollectionParameter<Address[]>("collOfAddressItems");

            test.Parameter<UInt16>("uint16Param");
            test.Parameter<UInt32>("uint32Param");
            test.Parameter<UInt64>("uint64Param");

            test.Parameter<double>("doubleParam");
            test.CollectionParameter<double>("collofDoubleParam");

            test.Parameter<float>("floatParam");
            test.CollectionParameter<float>("collofFloatParam");

            test.Parameter<Nullable<decimal>>("decimalParam");
            test.CollectionParameter<decimal>("collofDecimalParam");

            test.Parameter<Nullable<DateTime>>("dateParam");
            test.CollectionParameter<DateTime>("collofDateParam");

            test.Parameter<Nullable<DateTimeOffset>>("dateOffsetParam");
            test.CollectionParameter<DateTimeOffset>("collofDateOffsetParam");

            test.Parameter<Nullable<TimeSpan>>("timespanParam");
            test.CollectionParameter<TimeSpan>("collOftimespanParam");

            //test.Parameter<Uri>("uriParam");
            //test.CollectionParameter<Uri>("collOfUriParam");

            return modelBuilder.GetEdmModel();
        }
    }

    [Flags]
    public enum ClassType
    {
        Undergrad = 1,
        Grad = 2,
        Other = 4
    }
}
