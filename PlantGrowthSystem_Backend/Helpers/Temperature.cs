﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class Temperature
    {
        public Temperature(float temp, string date)
        {
            this._Temperature = temp;
            Date = date;
        }

        public float _Temperature { get; set; }
        public string Date { get; set; }
    }
}