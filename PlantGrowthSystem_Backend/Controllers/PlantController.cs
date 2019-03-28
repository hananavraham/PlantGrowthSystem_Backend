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
                    .Set("Min_temperature", plant.Min_temperature)
                    .Set("Max_temperature", plant.Max_temperature)
                    .Set("Min_light", plant.Min_light)
                    .Set("Max_light", plant.Max_light)
                    .Set("Min_humidity", plant.Min_humidity)
                    .Set("Max_humidity", plant.Max_humidity);
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
        //public ActionResult UpdateMeasure(string address, Measure measure)
        public ActionResult UpdateMeasure(string id, Measure measure)
        {
            try
            {
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));

                var filter = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var builder = Builders<PlantModel>.Update;
                var update = builder
                    .Push("Temperature", measure.Temp[0])
                    .Push("Light", measure.Light[0])
                    .Push("Humidity", measure.Humidity[0]);
                var result = plantCollection.UpdateOne(filter, update);
                return View();
            }

            catch
            {
                return View();
            }
        }

        // GET : Plant/UpdateSize
        // UpdateSize from measurement control unit
        [HttpGet]
        public ActionResult UpdateSize(string address, float size)
        {
            try
            {
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == address);
                var filter = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var update = Builders<PlantModel>.Update.Push("Size", new Size
                {
                    _Size = size,
                    Date = DateTime.Now.ToString()
                }
                    );
                var result = plantCollection.UpdateOne(filter, update);
                return View();
            }

            catch
            {
                return View();
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
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                var measure = new Measure();
                measure.Temp = plant.Temperature.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Light = plant.Light.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Humidity = plant.Humidity.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);

                return Content(JsonConvert.SerializeObject(measure));
            }

            catch
            {
                return View();
            }
        }

        // GET : Plant/GetIntervalsByDate
        // return the daily intervals values to the env. control unit
        [HttpGet]
        public ActionResult GetIntervalsByDate(string id, DateTime date)
        {
            try
            {
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));

                /* checking the research status if still running */
                var researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Plants_id.Contains(id));


                if (research.Status.Equals("Running"))
                {
                    Intervals interval = new Intervals();

                    interval.min_Light = plant.Min_light.Find(x => DateTime.Parse(x.Date) == date)._Light;
                    interval.max_Light = plant.Max_light.Find(x => DateTime.Parse(x.Date) == date)._Light;
                    interval.min_Humidity = plant.Min_humidity.Find(x => DateTime.Parse(x.Date) == date)._Humidity;
                    interval.max_Humidity = plant.Max_humidity.Find(x => DateTime.Parse(x.Date) == date)._Humidity;
                    interval.min_Temperature = plant.Min_temperature.Find(x => DateTime.Parse(x.Date) == date)._Temperature;
                    interval.max_Temperature = plant.Max_temperature.Find(x => DateTime.Parse(x.Date) == date)._Temperature;

                    return Content(JsonConvert.SerializeObject(interval));
                }


                // research status not "Running" - return null
                else
                    return null;



                //interval.min_Humidity = plant.Min_humidity.AsQueryable<Humidity>().SingleOrDefault(x => x.Date == date)._Humidity;
                //interval.max_Humidity = plant.Max_humidity.AsQueryable<Humidity>().SingleOrDefault(x => x.Date == date)._Humidity;
                //interval.min_Temperature = plant.Min_temperature.AsQueryable<Temperature>().SingleOrDefault(x => x.Date == date)._Temperature;
                //interval.max_Temperature = plant.Max_temperature.AsQueryable<Temperature>().SingleOrDefault(x => x.Date == date)._Temperature;
                //interval.min_Light = plant.Min_light.AsQueryable<Light>().SingleOrDefault(x => x.Date == date)._Light;
                //interval.max_Light = plant.Max_light.AsQueryable<Light>().SingleOrDefault(x => x.Date == date)._Light;


            }

            catch
            {
                return null;
            }

        }


    }
}