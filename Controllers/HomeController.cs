using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public HomeController(ILogger<HomeController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

            var user = new User()
            {
                Name = "sf",
                Password = "password",
            };
            _dbContext.UsersDB.Add(user);
            _dbContext.SaveChanges();

            var startup = new Startup()
            {
                Author = user,
                Name = "Home",
                Description = "Home324",
            };

            _dbContext.StartupsDB.Add(startup);
            _dbContext.SaveChanges();
        }

        public async Task<IActionResult> Index()
        {
            var users = _dbContext.UsersDB;

            return Json(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}