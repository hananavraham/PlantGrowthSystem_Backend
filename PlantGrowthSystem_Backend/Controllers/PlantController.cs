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
                return View();
            }

            catch
            {
                return View();
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

                    .Set("Intervals", plant.Intervals)
                    .Set("Frequency_of_measurement", plant.Frequency_of_measurement)
                    .Set("Frequency_of_upload", plant.Frequency_of_upload);
                var result = plantCollection.UpdateOne(filter, update);
                return View();
            }

            catch
            {
                return View();
            }
        }

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

        // GET : Plant/UpdateSize
        // UpdateSize from measurement control unit
        [HttpGet]
        public ActionResult UpdateSize(string id, float size)
        {
            try
            {
                var plant   = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                var filter  = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var update  = Builders<PlantModel>.Update.Push("Size", new Size
                {
                    _Size   = size,
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm")
                }
                    );
                var result  = plantCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(size));
            }

            catch
            {
                return null;
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

                /* checking the research status if still running */
                var researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Plants_id.Contains(id));


                if (research.Status.Equals("Running"))
                {
                    Intervals interval = new Intervals();
                    interval = plant.Intervals.AsQueryable<Intervals>().SingleOrDefault(x => x.date == date);
                    PlantDetails plantDetails = new PlantDetails()
                    {
                        Interavl = interval,
                        Frequency_of_measurement = plant.Frequency_of_measurement,
                        Frequency_of_upload = plant.Frequency_of_upload
                    };
                    return Content(JsonConvert.SerializeObject(plantDetails));
                }


                // research status not "Running" - return "stop" 
                else if(research.Status.Equals("Cancel") || research.Status.Equals("Stop") || research.Status.Equals("Finish"))
                    return Content(JsonConvert.SerializeObject("stop"));

                return null;
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }

        }


    }
}