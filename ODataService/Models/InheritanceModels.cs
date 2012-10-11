using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataService.Models
{
    //public abstract class Vehicle
    //{
    //    public int Id { get; set; }

    //    public string Model { get; set; }

    //    public string Name { get; set; }

    //    public abstract int WheelCount { get; }
    //}

    public class Vehicle
    {
        public int Id { get; set; }

        public string Model { get; set; }

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
    }

    public class SportBike : Motorcycle
    {
        public long TopSpeed { get; set; }
    }

    public class MotorcycleManufacturer
    {
        [Key]
        public string Name { get; set; }
    }
}
