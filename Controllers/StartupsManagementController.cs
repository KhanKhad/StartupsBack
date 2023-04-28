using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using StartupsBack.Utilities;
using StartupsBack.ViewModels;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using StartupsBack.ViewModels.ActionsResults;

namespace StartupsBack.Controllers
{
    public class StartupsManagementController : Controller
    {
        private readonly ILogger<StartupsManagementController> _logger;
        private readonly MainDb _dbContext;
        private readonly StartupsManagementViewModel _startupsManager;
        private readonly string[] _permittedExtensions = { ".png", ".jpg", ".jpeg" };
        private readonly long _fileSizeLimit;
        public StartupsManagementController(ILogger<StartupsManagementController> logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
            _fileSizeLimit = 1048576 * 3;
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

        [HttpPost]
        public async Task<IActionResult> CreateStartupFromMultiform()
        {
            var startupParseResult = await MultipartRequestHelper.GetStartupModelFromMultipart(Request, ModelState, _permittedExtensions, _fileSizeLimit);

            if (startupParseResult.StartupOrNull == null)
            {
                if (startupParseResult.StartupParseResultType == StartupParseResultType.BadModel)
                {
                    var sb = new StringBuilder();
                    sb.AppendJoin(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));

                    return BadRequest(new { UserParseResult = startupParseResult.StartupParseResultType.ToString(), ErrorOrEmpty = sb.ToString() });
                }
                else
                    return BadRequest(new { UserParseResult = startupParseResult.StartupParseResultType.ToString(), ErrorOrEmpty = startupParseResult.ErrorOrNull == null ? string.Empty : startupParseResult.ErrorOrNull.Message });

            }

            var createStartupResult = await _startupsManager.CreateStartupAsync(startupParseResult.StartupOrNull, startupParseResult.AuthorNameOrEmpty, startupParseResult.StartupHash);

            return Json(new { Result = createStartupResult.StartupCreateResultType.ToString()});
        }
    }
}
