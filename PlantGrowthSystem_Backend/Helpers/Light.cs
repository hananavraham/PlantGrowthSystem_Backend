using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class Light
    {
        public Light(int light, string date)
        {
            this._Light = light;
            Date = date;
        }

        public int _Light { get; set; }
        public string Date { get; set; }
    }
}