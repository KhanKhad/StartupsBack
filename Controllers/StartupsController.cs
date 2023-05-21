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
using System.Xml.Linq;

namespace StartupsBack.Controllers
{
    public class StartupsController : Controller
    {
        private readonly ILogger<StartupsController> _logger;
        private readonly MainDb _dbContext;
        private readonly StartupsManagementViewModel _startupsManager;
        private readonly string[] _permittedExtensions = { ".png", ".jpg", ".jpeg" };
        private readonly long _fileSizeLimit;
        public StartupsController(ILogger<StartupsController> logger, MainDb dbContext) 
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

            return Json(new { Result = createStartupResult.StartupCreateResultType.ToString(), ErrorOrEmpty = createStartupResult.ErrorOrNull == null ? string.Empty : createStartupResult.ErrorOrNull.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetStartupsIds(int pageNumber, int pageSize)
        {
            var startupsIds = await _startupsManager.GetStartupsIds(pageNumber, pageSize);
            return Json(startupsIds);
        }

        [HttpGet]
        public async Task<IActionResult> GetStartupById(int id)
        {
            var startup = await _startupsManager.GetStartupModelAsync(id);

            if (startup == null)
            {
                return BadRequest(new { Result = "NotFound" });
            }

            return new MultiformActionResult(startup, false, true);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyStartupsIds(int id, int pageNumber, int pageSize)
        {
            var startupsIds = await _startupsManager.GetMyStartupsIds(id, pageNumber, pageSize);
            return Json(startupsIds);
        }
        public async Task<IActionResult> GetStartupsDelta(int id)
        {
            var getMessagesResult = await _startupsManager.GetStartupsDelta(id);

            if (getMessagesResult.Delta == -1)
                return BadRequest(new { Result = getMessagesResult.GetDeltaResultType.ToString(), ErrorOrEmpty = getMessagesResult.ErrorOrNull == null ? string.Empty : getMessagesResult.ErrorOrNull.Message });

            return Json(getMessagesResult.Delta);
        }

        public async Task<IActionResult> GetStartupsJoinRequestes(int id)
        {
            var userAuthenticateResult = await _startupsManager.GetStartupsJoinRequestes(id);
            return Json(userAuthenticateResult);
        }

        public async Task<IActionResult> TryToJoinToStartup(int id, string hash, int startupId)
        {
            var userAuthenticateResult = await _startupsManager.TryToJoinToStartup(id, hash, startupId);
            return Json(new { Result = userAuthenticateResult.JoinToStartupResultType.ToString(), ErrorOrEmpty = userAuthenticateResult.ErrorOrNull == null ? string.Empty : userAuthenticateResult.ErrorOrNull.Message });
        }

        public async Task<IActionResult> AcceptUserToStartup(int id, string hash, int startupId, int userid)
        {
            var userAuthenticateResult = await _startupsManager.AcceptUserToStartup(id, hash, startupId, userid);
            return Json(new { Result = userAuthenticateResult.AcceptUserResultType.ToString(), ErrorOrEmpty = userAuthenticateResult.ErrorOrNull == null ? string.Empty : userAuthenticateResult.ErrorOrNull.Message });
        }

        public async Task<IActionResult> RejectUserToStartup(int id, string hash, int startupId, int userid)
        {
            var userAuthenticateResult = await _startupsManager.RejectUserToStartup(id, hash, startupId, userid);
            return Json(new { Result = userAuthenticateResult.RejectUserResultType.ToString(), ErrorOrEmpty = userAuthenticateResult.ErrorOrNull == null ? string.Empty : userAuthenticateResult.ErrorOrNull.Message });
        }
    }
}
