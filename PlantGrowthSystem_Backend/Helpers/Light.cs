using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class Light
    {
        public Light(float light, string date)
        {
            this._Light = light;
            Date = date;
        }

        public float _Light { get; set; }
        public string Date { get; set; }
    }
}