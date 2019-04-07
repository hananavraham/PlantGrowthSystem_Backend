using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class PlantDetails
    {
        public float Frequency_of_measurement { get; set; }
        public float Frequency_of_upload { get; set; }
        public Intervals Interavl { get; set; }
    }
}