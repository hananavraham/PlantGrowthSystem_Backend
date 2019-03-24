using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PlantGrowthSystem_Backend.App_Start
{
    public class MongoDBContext
    {
        public IMongoClient client;
        public IMongoDatabase database;

        public MongoDBContext()
        {
            client = new MongoClient(ConfigurationManager.AppSettings["mongoDBHost"]);
            database = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]);
        }
    }
}