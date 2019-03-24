using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PlantGrowthSystem_Backend.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Models
{
    public class PlantModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Research_id")]
        public string ResearchId { get; set; }

        [BsonElement("Env_control_address")]
        public string Env_control_address { get; set; }

        [BsonElement("Growth_control_address")]
        public string Growth_control_address { get; set; }

        [BsonElement("Min_temperature")]
        public List<Temperature> Min_temperature { get; set; }
        //public List<float> Min_temperature { get; set; }

        [BsonElement("Max_temperature")]
        public List<Temperature> Max_temperature { get; set; }
        //public List<float> Max_temperature { get; set; }

        [BsonElement("Min_light")]
        public List<Light> Min_light { get; set; }
        //public List<float> Min_light { get; set; }

        [BsonElement("Max_light")]
        public List<Light> Max_light { get; set; }
        //public List<float> Max_light { get; set; }

        [BsonElement("Min_humidity")]
        public List<Humidity> Min_humidity { get; set; }
        //public List<int> Min_humidity { get; set; }

        [BsonElement("Max_humidity")]
        public List<Humidity> Max_humidity { get; set; }
        //public List<int> Max_humidity { get; set; }

        [BsonElement("Temperature")]
        public List<Temperature> Temperature { get; set; }

        [BsonElement("Light")]
        public List<Light> Light { get; set; }

        [BsonElement("Humidity")]
        public List<Humidity> Humidity { get; set; }

        [BsonElement("Size")]
        public List<Size> Plant_size { get; set; }
    }
}