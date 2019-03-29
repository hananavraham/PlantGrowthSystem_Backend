using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class Humidity
    {
        public Humidity(int humidity, string date)
        {
            this._Humidity = humidity;
            Date = date;
        }

        public int _Humidity { get; set; }
        public string Date { get; set; }
    }
}