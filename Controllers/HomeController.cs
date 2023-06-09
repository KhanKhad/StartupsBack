﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;
using StartupsBack.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
            var users = await _dbContext.UsersDB.Include(i => i.ContributingProjects).ToListAsync();
            return Json(users);
        }

        public async Task<IActionResult> Startups()
        {
            var startups = await _dbContext.StartupsDB.Include(i => i.Contributors).ToListAsync();
            return Json(startups);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}