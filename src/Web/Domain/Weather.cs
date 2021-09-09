using System;
using System.Collections.Generic;

#nullable disable

namespace Web.Domain
{
    public partial class Weather
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public double Temperature { get; set; }
    }
}
