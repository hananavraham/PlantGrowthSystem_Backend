using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PlantGrowthSystem_Backend.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Models
{
    public class ControlPlanModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Plant_id")]
        public string PlantId { get; set; }

        [BsonElement("Frequency_of_measurement")]
        public float Frequency_of_measurement { get; set; }

        [BsonElement("Frequency_of_upload")]
        public float Frequency_of_upload { get; set; }

        [BsonElement("Intervals")]
        public List<Intervals> Intervals { get; set; }
    }
}