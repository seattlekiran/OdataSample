using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ODataService.Models
{
    [ComplexType]
    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public SomeOtherType SomeOtherType { get; set; }
    }

    [ComplexType]
    public class SomeOtherType
    {
        [MaxLength(10)]
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }
    }
}
