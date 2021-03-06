﻿using MongoDB.Bson;
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
        [HttpGet]
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
                if (research.Plants_id == null)
                    research.Plants_id = new List<string>();
                if (research.Owners == null)
                    research.Owners = new List<string>();

                researchCollection.InsertOne(research);
                return Content(JsonConvert.SerializeObject(research));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // PUT : Research/Edit
        [HttpPut]
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

        // DELETE : Research/Delete
        [HttpDelete]
        public ActionResult Delete(string id)
        {
            var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
            try
            {
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                researchCollection.DeleteOne(Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                //deleting all the plants belnogs to the research
                var plantCollection = dBContext.database.GetCollection<ResearchModel>("Plant");
                foreach (var plantId in research.Plants_id)
                {
                    // delete plant's control plan
                    controlPlanCollection.DeleteOne(x => x.PlantId == plantId);

                    // delete research's plant
                    plantCollection.DeleteOne(x => x.Id == ObjectId.Parse(plantId));

                }
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


        /* need to check this API  */ 

        // GET : Research/GetResearchesByOwner
        [HttpGet]
        public ActionResult GetResearchesByOwner(string ownerId)
        {
            List<ResearchModel> re = researchCollection.AsQueryable<ResearchModel>().ToList();
            List<ResearchModel> res = new List<ResearchModel>();
            try
            {
                foreach (var r in re)
                {
                    var a = r.Owners.FindAll(x => x.Equals(ownerId));
                    if (a.Count > 0)
                        res.Add(r);
                }
                //var researches = researchCollection.AsQueryable<ResearchModel>().SelectMany(x => x.Owners.Find(s => s.Equals(ownerId)));
                //var researches1 = researchCollection.AsQueryable<ResearchModel>().SelectMany(x => x.Owners.FindAll(s => s.Equals(ownerId)));
                return Content(JsonConvert.SerializeObject(res));
            }

            catch(Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }

        }
        /* -------------------------------- */

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
        public string GetNewResearchByIp(string plantIp, string envStatus)
        {
            try
            {
                bool IsAllPlantsRunning = false;
                var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plantIp && x.Status == "Pending");
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(plant.ResearchId));
                if (research.Status == "Pending")   // check if we have new Research
                {
                    // update Plant status to Running
                    var filter = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                    if(envStatus == "Ok" || envStatus == "Error")
                    {
                        var update = Builders<PlantModel>.Update
                            .Set("Status", envStatus);
                        var result = plantCollection.UpdateOne(filter, update);
                    }

                    // checking if all plants is running
                    foreach (string p in research.Plants_id)
                    {
                        plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(p));
                        if (plant.Status == "Pending")
                        {
                            IsAllPlantsRunning = false;
                            break;
                        }

                        else
                            IsAllPlantsRunning = true;

                    }

                    // if all plants is running - change Research status to Running
                    if (IsAllPlantsRunning)
                    {
                        var filter1 = Builders<ResearchModel>.Filter.Eq("_id", research.Id);
                        var update1 = Builders<ResearchModel>.Update
                            .Set("Status", "Running");
                        var result = researchCollection.UpdateOne(filter1, update1);
                    }
                    //return Content(JsonConvert.SerializeObject(plant.Id));
                    return plant.Id.ToString();
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        // GET : Research/GetNewResearchByGrowthControlUnitIP
        [HttpGet]
        public string GetNewResearchByGrowthControlUnitIP(string ip)
        {
            try
            {
                var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Growth_control_address == ip && x.Status == "Pending" || x.Growth_control_address == ip && x.Status == "Running");
                return plant.Id.ToString();
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

                return Content(JsonConvert.SerializeObject("Plant was succesfully added to Research"));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }

        }

        // GET : Research/GetResearchPlants
        [HttpGet]
        public ActionResult GetResearchPlants(string researchId)
        {
            List<PlantModel> plants = new List<PlantModel>();
            try
            {
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(researchId));
                var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
                foreach (string plantId in research.Plants_id)
                {
                    plants.Add(plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(plantId)));
                }
            }

            catch
            {
                return null;
            }

            return Content(JsonConvert.SerializeObject(plants));
        }



        /* need to check it!!!!!! */

        // Update Control Plan to all Plants from Excel file
        // POST : Research/UpdateIntervalsToPlants
        //[HttpPost]
        //public ActionResult UpdateIntervalsToPlants(string researchId, HttpPostedFileBase file)
        //{
        //    ReadExcelFile excel = new ReadExcelFile();

        //    /* need to get the file from POST */
        //    ListDictionary plantIntervals = excel.ParseExcel(file.FileName);

        //    var plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
        //    var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");

        //    foreach (KeyValuePair<string, List<Intervals>> plantInterval in plantIntervals)
        //    {
        //        var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plantInterval.Key);
        //        var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => ObjectId.Parse(x.PlantId) == plant.Id);

        //        var filter = Builders<ControlPlanModel>.Filter.Eq("_id", controlPlan.Id);
        //        var update = Builders<ControlPlanModel>.Update

        //            .Set("Intervals", controlPlan.Intervals);
        //        var result = controlPlanCollection.UpdateOne(filter, update);
        //    }

        //    return null;
        //}

    }
}