using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.JsonModels;
using StartupsBack.ViewModels;
using System.Text.Json;

namespace StartupsBack.Controllers
{
    public class StartupsManagementController : Controller
    {
        private readonly ILogger<StartupsManagementController> _logger;
        private readonly MainDb _dbContext;
        private readonly StartupsManagementViewModel _startupsManager;
        public StartupsManagementController(ILogger<StartupsManagementController> logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
            _startupsManager = new StartupsManagementViewModel(_logger, _dbContext);
        }
        [HttpPost]
        public async Task<IActionResult> CreateStartup()
        {
            var jsonoptions = new JsonSerializerOptions();
            jsonoptions.Converters.Add(new StartupJsonModelConverter());
            var startupModel = await Request.ReadFromJsonAsync<StartupJsonModel>(jsonoptions);
            if (startupModel == null) return BadRequest("startupModel undefined");

            var createUserResult = await _startupsManager.CreateStartupAsync(startupModel);

            var answer = new StartupJsonModel(createUserResult);
            var str = JsonSerializer.Serialize(answer, jsonoptions);

            return new OkObjectResult(str);
        }
    }
}
