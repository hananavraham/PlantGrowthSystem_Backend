using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class FullPlant
    {
        public string ResearchId { get; set; }
        public string Status { get; set; }
        public string Env_control_address { get; set; }
        public string Growth_control_address { get; set; }
        public float Frequency_of_measurement { get; set; }
        public float Frequency_of_upload { get; set; }
        public List<Temperature> Temperature { get; set; }
        public List<Light> Light { get; set; }
        public List<Humidity> Humidity { get; set; }
        public List<Size> Plant_size { get; set; }
        public List<Intervals> Intervals { get; set; }
    }
}