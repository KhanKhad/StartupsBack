using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupsBack.Database;
using StartupsBack.Models;
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
        }

        public async Task Index()
        {
            Response.ContentType = "text/html;charset=utf-8";
            await Response.WriteAsync(_dbContext.UserDB.ToList().Count.ToString());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}