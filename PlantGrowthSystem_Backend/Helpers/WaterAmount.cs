using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class WaterAmount
    {
        public WaterAmount(Double waterAmount, string date)
        {
            this.waterAmount = waterAmount;
            Date = date;
        }

        public Double waterAmount { get; set; }
        public string Date { get; set; }
    }
}