using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StartupsBack.Database;
using StartupsBack.Models;
using System;
using System.Diagnostics;

namespace StartupsBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MainDb _dbContext;
        private readonly MongoClient _dbMongo;
        public HomeController(ILogger<HomeController> logger, MainDb dbContext, MongoClient client)
        {
            _logger = logger;
            _dbContext = dbContext;
            _dbMongo = client;
            //HttpContext.Response.Cookies.Append("LastVisit", DateTime.Now.ToString("dd/MM/yyyy hh-mm-ss"));
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index");
            var users = await _dbContext.UsersDB.Include(x => x.PublishedStartups).Include(x => x.History).ToListAsync();
            var startups = await _dbContext.StartupsDB.ToListAsync();
            var t = users.FirstOrDefault(i => i.Id == 1);

            return Json(users);
        }


        public async Task<IActionResult> Users()
        {
            var users = await _dbContext.UsersDB.ToListAsync();
            return Json(users);
        }

        public async Task<IActionResult> Startups()
        {
            var startups = await _dbContext.StartupsDB.ToListAsync();
            return Json(startups);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}