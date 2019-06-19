using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class Intervals
    {
        public string date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public int min_Humidity { get; set; }
        public int max_Humidity { get; set; }
        //public float min_Temperature { get; set; }
        //public float max_Temperature { get; set; }
        public int light_cycle { get; set; } // in Hours
        //public int max_Light { get; set; }
    }
}