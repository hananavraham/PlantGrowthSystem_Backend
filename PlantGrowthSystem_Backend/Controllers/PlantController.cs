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
using System.Web.Http.Cors;
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
                return Content(JsonConvert.SerializeObject(plant));
            }

            catch
            {
                return null;
            }
        }

        // PUT : Plant/Edit
        // Edit from Client side
        [HttpPut]
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

        // PUT : Plant/UpdateMeasure
        // UpdateMeasure from enviroment control unit
        [HttpPut]
        public ActionResult UpdateMeasure(string id, Measure measure)
        {
            try
            {
                string date = DateTime.Now.ToString(("MM/dd/yyyy HH:mm"));
                var plant   = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));

                var filter  = Builders<PlantModel>.Filter.Eq("_id", plant.Id);
                var builder = Builders<PlantModel>.Update;
                var update = builder
                    //.Push("Temperature", new Temperature(measure.Temp, date))
                    .Push("Light", new Light(measure.Light, date))
                    .Push("Humidity", new Humidity(measure.Humidity, date))
                    .Push("WaterAmount", new WaterAmount(measure.WaterAmount, date))
                    .Push("PowerConsumption", new PowerConsumption(measure.PowerConsumption, date));
                var result  = plantCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(measure));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // PUT : Plant/UpdateSize
        // UpdateSize from measurement control unit
        [HttpPut]
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
                //measure.Temp     = plant.Temperature.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Light    = plant.Light.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Humidity = plant.Humidity.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Water = plant.WaterAmount.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);
                measure.Power = plant.PowerConsumption.FindAll(d => DateTime.Parse(d.Date) >= start_date && DateTime.Parse(d.Date) <= end_date);

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
            string date = DateTime.Now.ToString("M/dd/yyyy");

            try
            {
                var plant = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));

                var researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");
                var research = researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(plant.ResearchId));

                if (research.Status.Equals("Running"))
                {
                    DateTime end_date = Convert.ToDateTime(research.End_date);
                    DateTime now = Convert.ToDateTime(date);

                    //checking if we rich the end_date 
                    if(end_date.CompareTo(now) == -1)
                    {
                        // update research status to "complete"
                        var filter = Builders<ResearchModel>.Filter.Eq("_id", research.Id);
                        var update = Builders<ResearchModel>.Update
                            .Set("Status", "Complete");
                        var result = researchCollection.UpdateOne(filter, update);

                        var filter1 = Builders<PlantModel>.Filter.Eq("ResearchId", research.Id);
                        var update1 = Builders<PlantModel>.Update
                            .Set("Status", "Complete");
                        plantCollection.UpdateMany(filter1, update1);

                        return Content(JsonConvert.SerializeObject("stop"));
                    }

                    else
                    {
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
                    }

                }

                // plant status not "Running" - return "stop" 
                else if(plant.Status.Equals("Cancel") || plant.Status.Equals("Stop") || plant.Status.Equals("Complete"))
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

        // POST : Plant/CreatePlantAndAddControlPlant
        //[HttpPost]
        //public ActionResult CreatePlantAndAddControlPlant(string researchId, HttpPostedFileBase file)
        //{
        //    ReadExcelFile excel = new ReadExcelFile();

        //    string path = Server.MapPath("~/Content/" + file.FileName + DateTime.Now);
        //    file.SaveAs(path);
        //    List<FullPlant> plants = excel.ParseExcel(path);
        //    var researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");
        //    var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
        //    foreach(FullPlant plant in plants)
        //    {
        //        Create(researchId, new PlantModel {
        //            Env_control_address = plant.Env_control_address,
        //            Growth_control_address = plant.Growth_control_address,
        //            Frequency_of_measurement = plant.Frequency_of_measurement,
        //            Frequency_of_upload = plant.Frequency_of_upload,
        //            Status = plant.Status
        //        });


        //        var plan = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plant.Env_control_address);
        //        var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => ObjectId.Parse(x.PlantId) == plan.Id);

        //        // adding plant to research
        //        try
        //        {
        //            var filter = Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(researchId));
        //            var update = Builders<ResearchModel>.Update
        //                .Push("Plants", plan.Id);
        //            var result = researchCollection.UpdateOne(filter, update);
        //        }

        //        catch { }



        //        controlPlanCollection.InsertOne(new ControlPlanModel
        //        {
        //            Frequency_of_measurement = plant.Frequency_of_measurement,
        //            Frequency_of_upload = plant.Frequency_of_upload,
        //            Intervals = plant.Intervals,
        //            PlantId = plan.Id.ToString()
        //        });
        //    }

        //    return Content(JsonConvert.SerializeObject("success"));
        //}

        // POST : Plant/createPlantAndAddControlPlan
        [HttpPost]
        public ActionResult createPlantAndAddControlPlan(string researchId, FullPlant plant)
        {
            try
            {
                // checking if already plant with the same IP exsist and Running or Pending
                var plan = plantCollection.AsQueryable<PlantModel>().First(x => x.Env_control_address == plant.Env_control_address && x.Status == "Pending" ||
                    x.Env_control_address == plant.Env_control_address && x.Status == "Running");

                if (plan != null)
                {
                    // return error - will not create another plant
                    return Content(JsonConvert.SerializeObject(
                            "ip already exsist and running or pending"
                        ));
                }
            }

            catch { }
            try
            {
                var researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");
                var controlPlanCollection = dBContext.database.GetCollection<ControlPlanModel>("ControlPlan");
                Create(researchId, new PlantModel
                {
                    Plant_Name = plant.Plant_Name,
                    Env_control_address = plant.Env_control_address,
                    Growth_control_address = plant.Growth_control_address,
                    Frequency_of_measurement = plant.Frequency_of_measurement,
                    Frequency_of_upload = plant.Frequency_of_upload,
                    Status = "Pending",
                    Humidity = new List<Humidity>(),
                    Plant_size = new List<Size>(),
                    Light = new List<Light>(),
                    PowerConsumption = new List<PowerConsumption>(),
                    WaterAmount = new List<WaterAmount>()
                });

                var plan = plantCollection.AsQueryable<PlantModel>().SingleOrDefault(x => x.Env_control_address == plant.Env_control_address && x.Status == "Pending");
                //var controlPlan = controlPlanCollection.AsQueryable<ControlPlanModel>().SingleOrDefault(x => ObjectId.Parse(x.PlantId) == plan.Id);
                var filter = Builders<ResearchModel>.Filter.Eq("_id", ObjectId.Parse(researchId));
                var update = Builders<ResearchModel>.Update
                    .Push("Plants", plan.Id.ToString());
                var result = researchCollection.UpdateOne(filter, update);

                controlPlanCollection.InsertOne(new ControlPlanModel
                {
                    Frequency_of_measurement = plant.Frequency_of_measurement,
                    Frequency_of_upload = plant.Frequency_of_upload,
                    Intervals = plant.Intervals,
                    PlantId = plan.Id.ToString()
                });

            }

            catch { }

            return Content(JsonConvert.SerializeObject("success"));
        }

        // Get : Plant/plantPreTest
        [HttpGet]
        public string plantPreTest(string plantId, string status)
        {
            try
            {
                var filter = Builders<PlantModel>.Filter.Eq("_id", ObjectId.Parse(plantId));
                if (status == "Ok") 
                {
                    var update = Builders<PlantModel>.Update
                        .Set("Status", "Running");
                    var result = plantCollection.UpdateOne(filter, update);
                }

                else if(status == "Error")
                {

                }

                return "ok";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        // Get : Plant/UpdateStatus
        [HttpGet]
        public string UpdateStatus(string plantId, string status)
        {
            try
            {
                var filter = Builders<PlantModel>.Filter.Eq("_id", ObjectId.Parse(plantId));
                var update = Builders<PlantModel>.Update
                    .Set("Status", status);
                var result = plantCollection.UpdateOne(filter, update);


                return result.ToString();
            }

            catch(Exception e)
            {
                return e.Message;
            }

        }
    }
}