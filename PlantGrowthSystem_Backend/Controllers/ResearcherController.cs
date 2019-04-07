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
    public class ResearcherController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<ResearcherModel> researcherCollection;

        public ResearcherController()
        {
            dBContext = new MongoDBContext();
            researcherCollection = dBContext.database.GetCollection<ResearcherModel>("Researcher");
        }


        // GET: Researcher
        public ActionResult Index()
        {
            try
            {
                List<ResearcherModel> researches = researcherCollection.AsQueryable<ResearcherModel>().ToList();
                return Content(JsonConvert.SerializeObject(researches));
            }

            catch
            {
                return null;
            }

        }

        // GET : Researcher/GetResearcherById
        [HttpGet]
        public ActionResult GetResearcherById(string id)
        {
            try
            {
                var researcherId = new ObjectId(id);
                var researcher = researcherCollection.AsQueryable<ResearcherModel>().SingleOrDefault(x => x.Id == researcherId);
                return Content(JsonConvert.SerializeObject(researcher));
            }

            catch
            {
                return null;
            }
        }

        // POST : Researcher/GetResearcherByEmail
        [HttpPost]
        public ActionResult GetResearcherByEmail(string email)
        {
            try
            {
                var researcher = researcherCollection.AsQueryable<ResearcherModel>().SingleOrDefault(x => x.Email == email);
                return Content(JsonConvert.SerializeObject(researcher));
            }

            catch
            {
                return null;
            }
        }

        // POST : Researcher/Edit
        [HttpPost]
        public ActionResult Edit(ResearcherModel researcher)
        {
            try
            {
                var filter = Builders<ResearcherModel>.Filter.Eq("_id", researcher.Id);
                var update = Builders<ResearcherModel>.Update
                    .Set("Email", researcher.Email)
                    .Set("Degree", researcher.Degree);
                var result = researcherCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(researcher));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }

        }

        // POST : Researcher/Create
        [HttpPost]
        public ActionResult Create(ResearcherModel researcher)
        {
            try
            {
                researcherCollection.InsertOne(researcher);
                return Content(JsonConvert.SerializeObject(researcher));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // GET : Researcher/GetOwnerResearches
        [HttpGet]
        public ActionResult GetOwnerResearches(string id)
        {
            List<ResearchModel> researches = new List<ResearchModel>();

            try
            {
                var owner = researcherCollection.AsQueryable<ResearcherModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                var researchCollection = dBContext.database.GetCollection<ResearchModel>("Research");

                foreach (string researchId in owner.Researches_Id)
                {
                    researches.Add(researchCollection.AsQueryable<ResearchModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(researchId)));
                }

                return Content(JsonConvert.SerializeObject(researches));
            }

            catch
            {
                return null;
            }

        }

    }
}