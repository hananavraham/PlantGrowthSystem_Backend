using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlantGrowthSystem_Backend.App_Start;
using PlantGrowthSystem_Backend.Helpers;
using PlantGrowthSystem_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace PlantGrowthSystem_Backend.Controllers
{
    public class ControlPlanController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<ControlPlanModel> controlPlanCollection;

        public ControlPlanController()
        {
            dBContext = new MongoDBContext();
            controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
        }

        // GET: ControlPlan
        public ActionResult Index()
        {
            try
            {
                List<ControlPlanModel> controlPlans = controlPlanCollection.AsQueryable<ControlPlanModel>().ToList();
                return Content(JsonConvert.SerializeObject(controlPlans));
            }

            catch
            {
                return null;
            }

        }

        //GET : ControlPlan/GetControlPlanById
        [HttpGet]
        public ActionResult GetControlPlanById(string id)
        {
            try
            {
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                return Content(JsonConvert.SerializeObject(controlPlan));
            }

            catch
            {
                return null;
            }

        }

        //GET : ControlPlan/GetControlPlanByPlantId
        [HttpGet]
        public ActionResult GetControlPlanByPlantId(string plantId)
        {
            try
            {
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == plantId);
                return Content(JsonConvert.SerializeObject(controlPlan));
            }

            catch
            {
                return null;
            }

        }

        // POST : ControlPlan/Create
        // Create from Client side
        [HttpPost]
        public ActionResult Create(ControlPlanModel controlPlan)
        {
            try
            {
                controlPlanCollection.InsertOne(controlPlan);
                return Content(JsonConvert.SerializeObject("ok"));
            }

            catch
            {
                return null;
            }
        }

        // POST : ControlPlan/Edit
        [HttpPost]
        public ActionResult Edit(ControlPlanModel controlPlan)
        {
            try
            {
                var filter = Builders<ControlPlanModel>.Filter.Eq("PlantId", controlPlan.PlantId);
                var update = Builders<ControlPlanModel>.Update
                    .Set("Intervals", controlPlan.Intervals)                   
                    .Set("Frequency_of_measurement", controlPlan.Frequency_of_measurement)
                    .Set("Frequency_of_upload", controlPlan.Frequency_of_upload);
                var result = controlPlanCollection.UpdateOne(filter, update);

                return Content(JsonConvert.SerializeObject(controlPlan));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : ControlPlan/UpdateFrequencyOfMeasurement
        [HttpGet]
        public ActionResult UpdateFrequencyOfMeasurement(string plantId, float freq)
        {
            try
            {
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == plantId);
                var filter = Builders<ControlPlanModel>.Filter.Eq("_id", controlPlan.Id);
                var update = Builders<ControlPlanModel>.Update
                    .Set("Frequency_of_measurement", freq);
                var result = controlPlanCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(controlPlan));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : ControlPlan/UpdateFrequencyOfUpload
        [HttpGet]
        public ActionResult UpdateFrequencyOfUpload(string plantId, float freq)
        {
            try
            {
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == plantId);
                var filter = Builders<ControlPlanModel>.Filter.Eq("_id", controlPlan.Id);
                var update = Builders<ControlPlanModel>.Update
                    .Set("Frequency_of_upload", freq);
                var result = controlPlanCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(controlPlan));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET: ControlPlan/Delete
        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                controlPlanCollection.DeleteOne(Builders<ControlPlanModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                return Content(JsonConvert.SerializeObject("Control Plan deleted"));

            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : ControlPlan/GetControlPlanIntervalByPlantId
        public ActionResult GetControlPlanIntervalByPlantId(string plantId)
        {
            string date = DateTime.Now.ToString("MM/dd/yyyy");
            try
            {
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == plantId);
                var interval = controlPlan.Intervals.AsQueryable<Intervals>().SingleOrDefault(x => x.date == date);
                PlantDetails plantDetails = new PlantDetails()
                {
                    Control_plan = interval,
                    Frequency_of_measurement = controlPlan.Frequency_of_measurement,
                    Frequency_of_upload = controlPlan.Frequency_of_upload
                };
                return Content(JsonConvert.SerializeObject(plantDetails));
            }

            catch(Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // POST : ControlPlan/UpdateControlPlan
        [HttpPost]
        public ActionResult UpdateControlPlan(string plantId, List<Intervals> intervals)
        {
            try
            {
                var filter = Builders<ControlPlanModel>.Filter.Eq("PlantId", plantId);
                var update = Builders<ControlPlanModel>.Update
                    .Set("Intervals", intervals);
                var result = controlPlanCollection.UpdateOne(filter, update);

                return Content(JsonConvert.SerializeObject(result));
            }

            catch(Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }


        }

    }
}