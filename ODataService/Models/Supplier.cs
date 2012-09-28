using System.Collections.Generic;
using System;

namespace ODataService.Models
{
    public class Supplier
    {
        public Supplier()
        {
            ProductFamilies = new List<ProductFamily>();
            Addresses = new List<Address>();
            Tags = new List<string>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public ICollection<string> Tags { get; set; }

        public virtual ICollection<ProductFamily> ProductFamilies { get; set; }

        public Country? Country { get; set; }
    }

    [Flags]
    public enum Country
    { 
        USA = 1,
        China = 2,
        Japen = 4,
        Canada = 8,
        India = 16
    }
}
