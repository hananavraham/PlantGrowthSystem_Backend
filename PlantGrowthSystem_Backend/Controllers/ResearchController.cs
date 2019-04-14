using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlantGrowthSystem_Backend.App_Start;
using PlantGrowthSystem_Backend.Helpers;
using PlantGrowthSystem_Backend.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantGrowthSystem_Backend.Controllers
{
    public class ResearchController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<ResearchModel> researchCollection;

        public ResearchController()
        {
            dBContext = new MongoDBContext();
            researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");
        }



        // GET: Research
        public ActionResult Index()
        {
            try
            {
                List<ResearchModel> researches = researchCollection.AsQueryable<ResearchModel>().ToList();
                return Content(JsonConvert.SerializeObject(researches));
            }
            catch
            {
                return null;
            }

        }

        //GET : Research/GetResearchById
        public ActionResult GetResearchById(string id)
        {
            try
            {
                var researchId = new ObjectId(id);
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == researchId);
                return Content(JsonConvert.SerializeObject(research));
            }

            catch
            {
                return null;
            }

        }


        // POST : Research/Create
        [HttpPost]
        public ActionResult Create(ResearchModel research)
        {
            try
            {
                //research.Status = "Pending";
                researchCollection.InsertOne(research);
                return Content(JsonConvert.SerializeObject(research));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // POST : Research/Edit
        [HttpPost]
        public ActionResult Edit(ResearchModel research)
        {
            try
            {
                var filter = Builders<ResearchModel>.Filter.Eq("_id", research.Id);
                var update = Builders<ResearchModel>.Update
                    .Set("Start_date", research.Start_date)
                    .Set("End_date", research.End_date)
                    .Set("Owners", research.Owners);
                var result = researchCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(research));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : Research/Delete
        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                researchCollection.DeleteOne(Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                //deleting all the plants belnogs to the research
                var plantCollection = dBContext.database.GetCollection<ResearchModel>("Plant");
                foreach (var plantId in research.Plants_id)
                    plantCollection.DeleteOne(x => x.Id == ObjectId.Parse(plantId));
                return Content(JsonConvert.SerializeObject("Research deleted"));
            }

            catch
            {
                return null;
            }

        }

        // GET : Research/AddOwnersToResearch 
        [HttpGet]
        public ActionResult AddOwnersToResearch(string researchId, List<string> ownersId)
        {
            try
            {
                var filter = Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(researchId));
                var builder = Builders<ResearchModel>.Update;
                var update = builder
                    .Push("Owners", ownersId);
                var result = researchCollection.UpdateOne(filter, update);
                return View();
            }

            catch
            {
                return null;
            }

        }

        // GET : Research/GetResearchesByOwner
        [HttpGet]
        public ActionResult GetResearchesByOwner(string ownerId)
        {
            //var researches = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Owners == ObjectId.Parse(ownerId));
            //return Content(JsonConvert.SerializeObject(researchs));
            return View();
        }


        // GET : Research/StopOrContinueResearch
        [HttpGet]
        public ActionResult StopOrContinueResearch(string id, string status)
        {
            try
            {
                var filter = Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<ResearchModel>.Update
                    .Set("Status", status);
                var result = researchCollection.UpdateOne(filter, update);

                var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
                var filter1 = Builders<PlantModel>.Filter.Eq("ResearchId",id);
                var update1 = Builders<PlantModel>.Update
                    .Set("Status", status);
                plantCollection.UpdateMany(filter1, update1);
                return Content(JsonConvert.SerializeObject("Research status has been change"));
            }

            catch
            {
                return null;
            }
        }

        // This method check if we have new Research in pending
        // return plant Model to env. control ip
        // GET : Research/GetNewResearchByIp
        [HttpGet]
        public ActionResult GetNewResearchByIp(string plantIp)
        {
            try
            {
                bool IsAllPlantsRunning = false;
                var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plantIp);
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(plant.ResearchId));
                if (research.Status == "Pending")   // check if we have new Research
                {
                    // update Plant status to Running
                    var filter = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                    var update = Builders<PlantModel>.Update
                        .Set("Status", "Running");
                    var result = plantCollection.UpdateOne(filter, update);

                    // checking if all plants is running
                    foreach (string p in research.Plants_id)
                    {
                        plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plantIp);
                        if (plant.Status != "Running")
                        {
                            IsAllPlantsRunning = false;
                            break;
                        }

                        else
                            IsAllPlantsRunning = true;

                    }

                    // if all plants is running - change Research status to Running
                    if(IsAllPlantsRunning)
                    {
                        var filter1 = Builders<ResearchModel>.Filter.Eq("_id", research.Id);
                        var update1 = Builders<ResearchModel>.Update
                            .Set("Status", "Running");
                        result = researchCollection.UpdateOne(filter1, update1);
                    }
                    return Content(JsonConvert.SerializeObject(plant.Id));
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        // GET : Research/AddPlantToResearch
        [HttpGet]
        public ActionResult AddPlantToResearch(string researchId, string plantId)
        {
            try
            {
                var filter = Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(researchId));
                var update = Builders<ResearchModel>.Update
                    .Push("Plants", plantId);
                var result = researchCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject("Plant was added to Research succesfully"));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }

        }



        /* need to check it!!!!!! */

        // Update Control Plan to all Plants from Excel file
        // POST : Research/UpdateIntervalsToPlants
        [HttpPost]
        public ActionResult UpdateIntervalsToPlants(string researchId, HttpPostedFileBase file)
        {
            ReadExcelFile excel = new ReadExcelFile();

            /* need to get the file from POST */
            ListDictionary plantIntervals = excel.ParseExcel(file.FileName);

            var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
            var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");

            foreach (KeyValuePair<string, List<Intervals>> plantInterval in plantIntervals)
            {
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plantInterval.Key);
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => ObjectId.Parse(x.PlantId) == plant.Id);

                var filter = Builders<ControlPlanModel>.Filter.Eq("_id", controlPlan.Id);
                var update = Builders<ControlPlanModel>.Update

                    .Set("Intervals", controlPlan.Intervals);
                var result = controlPlanCollection.UpdateOne(filter, update);
            }

            return null;
        }

    }
}