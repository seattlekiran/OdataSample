using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QueryableRepro.Models
{
    public enum Enum1
    {
        One, 
        Two,
        Three
    }

    public enum Enum2
    {
        One,
        Two,
        Three
    }

    [Flags]
    public enum Enum3
    {
        One,
        Two,
        Three
    }

    public class Model
    {
        public Enum1 Enum1 { get; set; }
        public Enum1? Enum10 { get; set; }
        public Enum2 Enum2 { get; set; }
        public Enum3 Enum3 { get; set; }
    }
}