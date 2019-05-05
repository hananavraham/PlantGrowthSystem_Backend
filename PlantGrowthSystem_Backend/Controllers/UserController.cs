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
    public class UserController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<UserModel> userCollection;

        public UserController()
        {
            dBContext = new MongoDBContext();
            userCollection = dBContext.database.GetCollection<UserModel>("User");
        }

        //GET : User/GetUserById
        [HttpGet]
        public ActionResult GetUserById(string id)
        {
            try
            {
                var user = userCollection.AsQueryable<UserModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
                return Content(JsonConvert.SerializeObject(user));
            }

            catch
            {
                return null;
            }

        }

        //GET : User/GetUserById
        [HttpGet]
        public ActionResult GetUserByEmail(string email)
        {
            try
            {
                var user = userCollection.AsQueryable<UserModel>().SingleOrDefault(x => x.Email == email);
                return Content(JsonConvert.SerializeObject(user));
            }

            catch
            {
                return null;
            }

        }

        // POST : User/Create
        [HttpPost]
        public ActionResult Create(UserModel user)
        {
            try
            {
                userCollection.InsertOne(user);
                return Content(JsonConvert.SerializeObject("ok"));
            }

            catch
            {
                return null;
            }
        }

        // POST : User/Edit
        [HttpPost]
        public ActionResult Edit(UserModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq("_id", user.Id);
                var update = Builders<UserModel>.Update
                    .Set("password", user.Password);
                var result = userCollection.UpdateOne(filter, update);
                return Content(JsonConvert.SerializeObject(user));
            }

            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(e.Message));
            }
        }

        // POST : User/CheckUserCredentials
        [HttpPost]
        public ActionResult CheckUserCredentials(UserModel user)
        {
            try
            {
                var verifyUser = userCollection.AsQueryable<UserModel>().SingleOrDefault(x => x.Email.ToLower() == user.Email.ToLower() && x.Password == user.Password);
                if (verifyUser != null)
                {
                    var researcherCollection = dBContext.database.GetCollection<ResearcherModel>("Researcher");
                    var researcher = researcherCollection.AsQueryable<ResearcherModel>().SingleOrDefault(x => x.Email == verifyUser.Email);
                    return Content(JsonConvert.SerializeObject(researcher.Id));
                }
            }

            catch { return null; }

            return null;

        }
    }
}