using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlantGrowthSystem_Backend.App_Start;
using PlantGrowthSystem_Backend.Helpers;
using PlantGrowthSystem_Backend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantGrowthSystem_Backend.Controllers
{
    public class PlantController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<PlantModel> plantCollection;

        public PlantController()
        {
            dBContext = new MongoDBContext();
            plantCollection = dBContext.database.GetCollection<PlantModel>("Plant");
        }



        // GET: Plant
        public ActionResult Index()
        {
            try
            {
                List<PlantModel> plants = plantCollection.AsQueryable<PlantModel>().ToList();
                return Content(JsonConvert.SerializeObject(plants));
            }

            catch
            {
                return null;
            }

        }

        //GET : Plant/GetPlantById
        [HttpGet]
        public ActionResult GetPlantById(string id)
        {
            try
            {
                var plantId = new ObjectId(id);
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == plantId);
                return Content(JsonConvert.SerializeObject(plant));
            }

            catch
            {
                return null;
            }

        }


        // POST : Plant/Create
        // Create from Client side
        [HttpPost]
        public ActionResult Create(string researchId, PlantModel plant)
        {
            try
            {
                plant.ResearchId = researchId;
                plantCollection.InsertOne(plant);
                return Content(JsonConvert.SerializeObject("ok"));
            }

            catch
            {
                return null;
            }
        }

        // POST : Plant/Edit
        // Edit from Client side
        [HttpPost]
        public ActionResult Edit(PlantModel plant)
        {
            try
            {
                var filter = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var update = Builders<PlantModel>.Update
                    .Set("Frequency_of_measurement", plant.Frequency_of_measurement)
                    .Set("Frequency_of_upload", plant.Frequency_of_upload);
                var result = plantCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(plant));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        //// GET : Plant/UpdateFrequencyOfMeasurement
        //[HttpGet]
        //public ActionResult UpdateFrequencyOfMeasurement(string plantId, float freq)
        //{
        //    try
        //    {
        //        var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
        //        var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == plantId);
        //        var filter = Builders<ControlPlanModel>.Filter.Eq("_id", controlPlan.Id);
        //        var update = Builders<ControlPlanModel>.Update
        //            .Set("Frequency_of_measurement", freq);
        //        var result = controlPlanCollection.UpdateOne(filter, update);
        //        return Content(JsonConvert.SerializeObject(controlPlan));
        //    }

        //    catch(Exception e)
        //    {
        //        return Content(JsonConvert.SerializeObject(e.Message));
        //    }
        //}

        //// GET : Plant/UpdateFrequencyOfUpload
        //[HttpGet]
        //public ActionResult UpdateFrequencyOfUpload(string plantId, float freq)
        //{
        //    try
        //    {
        //        var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
        //        var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == plantId);
        //        var filter = Builders<ControlPlanModel>.Filter.Eq("_id", controlPlan.Id);
        //        var update = Builders<ControlPlanModel>.Update
        //            .Set("Frequency_of_upload", freq);
        //        var result = controlPlanCollection.UpdateOne(filter, update);
        //        return Content(JsonConvert.SerializeObject(controlPlan));
        //    }

        //    catch (Exception e)
        //    {
        //        return Content(JsonConvert.SerializeObject(e.Message));
        //    }
        //}

        // POST : Plant/UpdateMeasure
        // UpdateMeasure from enviroment control unit
        [HttpPost]
        public ActionResult UpdateMeasure(string id, Measure measure)
        {
            try
            {
                string date = DateTime.Now.ToString(("MM/dd/yyyy HH:mm"));
                var plant   = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));

                var filter  = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var builder = Builders<PlantModel>.Update;
                var update  = builder
                    .Push("Temperature", new Temperature(measure.Temp, date))
                    .Push("Light", new Light(measure.Light, date))
                    .Push("Humidity", new Humidity(measure.Humidity, date));
                var result  = plantCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(measure));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // POST : Plant/UpdateSize
        // UpdateSize from measurement control unit
        [HttpPost]
        public ActionResult UpdateSize(string id, Size size)
        {
            try
            {
                var plant   = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                var filter  = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var update  = Builders<PlantModel>.Update.Push("Size", new Size
                {
                    Height  = size.Height,
                    Volume  = size.Volume,
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm")
                }
                    );
                var result  = plantCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(plant));
            }

            catch(Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : Plant/GetMeasuresByRange
        // GetMeasuresByRange from Client side
        // return Mesaure - list of temp, light, humidity
        [HttpGet]
        public ActionResult GetMeasuresByRange(string id, DateTime start_date, DateTime end_date)
        {
            try
            {
                var plant        = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                var measure      = new Measures();
                measure.Temp     = plant.Temperature.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Light    = plant.Light.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Humidity = plant.Humidity.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);

                return Content(JsonConvert.SerializeObject(measure));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : Plant/GetIntervalsByDate
        // return the daily intervals values to the env. control unit
        [HttpGet]
        public ActionResult GetIntervalsByDate(string id)
        {
            string date = DateTime.Now.ToString("MM/dd/yyyy");
            try
            {
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));

                var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
                var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => x.PlantId == id);

                //checking the plant status if still running
                if (plant.Status.Equals("Running"))
                {
                    Intervals interval = new Intervals();
                    interval = controlPlan.Intervals.AsQueryable<Intervals>().SingleOrDefault(x => x.date == date);
                    PlantDetails plantDetails = new PlantDetails()
                    {
                        Control_plan = interval,
                        Frequency_of_measurement = plant.Frequency_of_measurement,
                        Frequency_of_upload = plant.Frequency_of_upload
                    };
                    return Content(JsonConvert.SerializeObject(plantDetails));
                }


                // plant status not "Running" - return "stop" 
                else if(plant.Status.Equals("Cancel") || plant.Status.Equals("Stop") || plant.Status.Equals("Finish"))
                    return Content(JsonConvert.SerializeObject("stop"));

                return null;
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }

        }

        // POST : Plant/CalculateImageSize
        [HttpPost]
        public ActionResult CalculateImageSize(StreamReader file)
        {

            return null;
        }


    }
}