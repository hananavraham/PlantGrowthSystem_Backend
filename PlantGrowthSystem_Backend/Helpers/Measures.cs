using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class Measures
    {
        //public List<Temperature> Temp { get; set; }
        public List<PowerConsumption> Power { get; set; }
        public List<WaterAmount> Water { get; set; }
        public List<Light> Light { get; set; }
        public List<Humidity> Humidity { get; set; }
    }
}