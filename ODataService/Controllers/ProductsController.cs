﻿using ODataService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder.Conventions;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller implements everything the OData Web API integration enables by hand.
    /// </summary>
    public class ProductsController : ApiController
    {
        // this example uses EntityFramework CodeFirst
        ProductsContext _db = new ProductsContext();

        /// <summary>
        /// Adds support for getting products, for example:
        /// 
        /// GET /Products
        /// GET /Products?$filter=Name eq 'Windows 95'
        /// GET /Products?
        /// 
        /// <remarks>
        /// Support for $filter, $orderby, $top and $skip is provided by the [Queryable] attribute.
        /// </remarks>
        /// </summary>
        /// <returns>An IQueryable with all the products you want it to be possible for clients to reach.</returns>
        [Queryable]
        public IQueryable<Product> GetProducts()
        {
            // If you have any security filters you should apply them before returning then from this method.
            return _db.Products;
        }

        /// <summary>
        /// Adds support for getting a product by key, for example:
        /// 
        /// GET /Products(1)
        /// </summary>
        /// <param name="id">The key of the Product required</param>
        /// <returns>The Product</returns>
        public HttpResponseMessage GetProductById(int id)
        {
            Product product = _db.Products.Where(p => p.ID == id).SingleOrDefault();
            if (product == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, product);
            }
        }

        /// <summary>
        /// Support for updating products
        /// </summary>
        public HttpResponseMessage Put(int id, Product update)
        {
            if (!_db.Products.Any(p => p.ID == id))
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            update.ID = id; // ignore the ID in the entity use the ID in the URL.

            _db.Products.Attach(update);
            _db.Entry(update).State = System.Data.EntityState.Modified;
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for creating products
        /// </summary>
        public HttpResponseMessage PostProduct(Product product)
        {
            product.Family = null;

            Product addedProduct = _db.Products.Add(product);
            _db.SaveChanges();
            var response = Request.CreateResponse(HttpStatusCode.Created, addedProduct);
            response.Headers.Location = new Uri(Url.Link(ODataRouteNames.GetById, new { Controller = "Products", Id = addedProduct.ID }));
            return response;
        }

        /// <summary>
        /// Support for partial updates of products
        /// </summary>
        public HttpResponseMessage PatchProduct(int id, Delta<Product> product)
        {
            Product dbProduct = _db.Products.SingleOrDefault(p => p.ID == id);
            if (dbProduct == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            product.Patch(dbProduct);
            _db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for deleting products by key.
        /// </summary>
        public HttpResponseMessage DeleteProduct(int id)
        {
            _db.Entry(_db.Products.Find(id)).State = EntityState.Deleted;
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Support for removing links between resources
        /// </summary>
        /// <param name="id">The key of the entity with the navigation property</param>
        /// <param name="navigationProperty">The navigation property on the entity to be modified</param>
        /// <param name="link">The url to the other entity that should no longer be linked to the entity via the navigation property</param>
        public HttpResponseMessage DeleteLink(int id, string navigationProperty, [FromBody] Uri link)
        {
            Product product = _db.Products.SingleOrDefault(p => p.ID == id);

            switch (navigationProperty)
            {
                case "Family":
                    product.Family = null;
                    break;

                default:
                    throw ODataErrors.DeletingLinkNotSupported(Request, navigationProperty);
            }
            _db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for creating links between entities in this entity set and other entities
        /// using the specified navigation property.
        /// </summary>
        /// <remarks>
        /// In this example Product only has a Product.Family relationship, which is a singleton, soon only PUT
        /// support is required, if there was a Product.Orders relationship - a collection - then this would need 
        /// to respond to POST requests too.
        /// </remarks>
        /// <param name="id">The key of the Entity in this EntitySet</param>
        /// <param name="navigationProperty">The navigation property of the Entity in this EntitySet that should be modified</param>
        /// <param name="link">The url to the other entity that should be related via the navigationProperty</param>
        public HttpResponseMessage PutLink(int id, string navigationProperty, [FromBody] Uri link)
        {
            Product product = _db.Products.SingleOrDefault(p => p.ID == id);

            switch (navigationProperty)
            {
                case "Family":
                    // The utility method uses routing (ODataRoutes.GetById should match) to get the value of {id} parameter 
                    // which is the id of the ProductFamily.
                    int relatedId = Configuration.GetKeyValue<int>(link);
                    ProductFamily family = _db.ProductFamilies.SingleOrDefault(f => f.ID == relatedId);
                    product.Family = family;
                    break;

                default:
                    throw ODataErrors.CreatingLinkNotSupported(Request, navigationProperty);
            }
            _db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds support for getting a ProductFamily from a Product, for example:
        /// 
        /// GET /Products(11)/Family
        /// </summary>
        /// <param name="parentId">The id of the Product</param>
        /// <returns>The related ProductFamily</returns>
        public ProductFamily GetFamily(int parentId)
        {
            //return null;
            return _db.Products.Where(p => p.ID == parentId).Select(p => p.Family).SingleOrDefault();
        }

        public Product ExtendSupportDate(int boundId, ODataActionParameters actionParams)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState));
            }

            Product prd = _db.Products.Where(p => p.ID == boundId).Single();

            ////prd.SupportedUntil = (DateTime)actionParams["newDate"];

            ////_db.SaveChanges();

            return prd;
        }

        public IEnumerable<Product> ExtendSupportDates(ODataActionParameters actionParams)
        {
            //Product prd = _db.Products.Where(p => p.ID == boundId).Single();

            ////prd.SupportedUntil = (DateTime)actionParams["newDate"];

            ////_db.SaveChanges();

            //return prd;

            return _db.Products;
        }

        public string DiscontinueProduct(ODataActionParameters actionParams)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            int productToDiscontinue = (int)actionParams["id"];

            //return _db.Products.Where(p => p.ID == productToDiscontinue).Single();
            return "success";
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
