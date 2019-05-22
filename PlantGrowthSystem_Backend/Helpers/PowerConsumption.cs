using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class PowerConsumption
    {
        public PowerConsumption(Double powerConsumption, string date)
        {
            this.powerConsumption = powerConsumption;
            Date = date;
        }

        public Double powerConsumption { get; set; }
        public string Date { get; set; }
    }
}