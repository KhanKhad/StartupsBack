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
                Name = "sf2",
                Password = "password",
            };
            var user2 = new User()
            {
                Name = "sf2",
                Password = "password",
            };

            var startup = new Startup()
            {
                Author = user,
                Name = "Home",
                Description = "Home324",
            };
            var startup2 = new Startup()
            {
                Author = user2,
                Name = "Home2",
                Description = "Home324",
            };
            user.History.Add(startup);
            user.History.Add(startup2);

            _dbContext.StartupsDB.AddRange(startup, startup2);
            _dbContext.UsersDB.AddRange(user, user2);

            _dbContext.SaveChanges();
        }

        public async Task<IActionResult> Index()
        {
            var users = _dbContext.UsersDB.Include(x => x.PublishedStartups).Include(x => x.History).ToList();
            var startups = _dbContext.StartupsDB.ToList();
            var t = users.FirstOrDefault(i => i.Id == 1);
            return Json(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}