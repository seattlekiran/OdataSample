using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ODataService.Models;
using System.Collections.Generic;
using System;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller implements support for Suppliers EntitySet.
    /// It does not implement everything, it only supports Query, Get by Key and Create, 
    /// by handling these requests:
    /// 
    /// GET /Suppliers
    /// GET /Suppliers(key)
    /// GET /Suppliers?$filter=..&$orderby=..&$top=..&$skip=..
    /// POST /Suppliers
    /// </summary>
    public class SuppliersController : EntitySetController<Supplier, int>
    {
        ProductsContext _db = new ProductsContext();

        public override IQueryable<Supplier> Get()
        {
            return _db.Suppliers;
        }

        protected override Supplier GetEntityById(int id)
        {
            return _db.Suppliers.SingleOrDefault(s => s.ID == id);
        }

        protected override Supplier CreateEntity(Supplier entity)
        {
            _db.Suppliers.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        protected override void DeleteEntity(int id)
        {
            var toDelete = _db.Suppliers.FirstOrDefault(s => s.ID == id);
            if (toDelete == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            _db.Suppliers.Remove(toDelete);
            _db.SaveChanges();
        }

        public ICollection<string> GetTags(int parentId)
        {
            Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == parentId);
            return supplier.Tags;
        }

        public ICollection<Address> GetAddresses(int parentId)
        {
            Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == parentId);
            return supplier.Addresses;
        }

        public string GetCountry(int parentId)
        {
            Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == parentId);
            return supplier.Country.ToString();
        }

        public void PutCountry(int parentId, [FromBody]string value)
        {
            Country country;
            if (Enum.TryParse<Country>(value, out country))
            {
                Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == parentId);
                supplier.Country = country;
                _db.SaveChanges();
            }
        }

        protected override int GetId(Supplier entity)
        {
            return entity.ID;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
