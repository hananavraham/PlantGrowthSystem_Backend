using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlantGrowthSystem_Backend.App_Start;
using PlantGrowthSystem_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantGrowthSystem_Backend.Controllers
{
    public class GeneralPlantController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<GeneralPlantModel> generalPlantCollection;

        public GeneralPlantController()
        {
            dBContext = new MongoDBContext();
            generalPlantCollection = dBContext.database.GetCollection<GeneralPlantModel>("General_Plant");
        }
        // GET: GeneralPlant
        public ActionResult Index()
        {
            try
            {
                List<GeneralPlantModel> generalPlants = generalPlantCollection.AsQueryable<GeneralPlantModel>().ToList();
                return Content(JsonConvert.SerializeObject(generalPlants));
            }

            catch
            {
                return null;
            }
        }

        //GET : GeneralPlant/GetGeneralPlantById
        [HttpGet]
        public ActionResult GetGeneralPlantById(string id)
        {
            try
            {
                var generalPlantId = new ObjectId(id);
                var generalPlant = generalPlantCollection.AsQueryable<GeneralPlantModel>().SingleOrDefault(x => x.Id == generalPlantId);
                return Content(JsonConvert.SerializeObject(generalPlant));
            }

            catch
            {
                return null;
            }

        }

        // POST : GeneralPlant/Create
        [HttpPost]
        public ActionResult Create(GeneralPlantModel generalPlant)
        {
            try
            {
                generalPlantCollection.InsertOne(generalPlant);
                return Content(JsonConvert.SerializeObject("ok"));
            }

            catch
            {
                return null;
            }
        }

        // POST : GeneralPlant/Edit
        [HttpPost]
        public ActionResult Edit(GeneralPlantModel generalPlant)
        {
            try
            {
                var filter = Builders<GeneralPlantModel>.Filter.Eq("_id", generalPlant.Id);
                var update = Builders<GeneralPlantModel>.Update
                    .Set("Name", generalPlant.Name)
                    .Set("Image", generalPlant.Image);
                var result = generalPlantCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(generalPlant));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }
    }
}