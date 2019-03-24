using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.Models
{
    public class ResearchModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Start_date")]
        public string Start_date { get; set; }

        [BsonElement("End_date")]
        public string End_date { get; set; }

        [BsonElement("General_plant_id")]
        public string General_plant_id { get; set; }

        [BsonElement("Number_of_plants")]
        public int Number_of_plants { get; set; }

        [BsonElement("Owners")]
        public List<string> Owners { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("Plants")]
        public List<string> Plants_id { get; set; }
    }
}