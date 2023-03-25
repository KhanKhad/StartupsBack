using Microsoft.AspNetCore.Mvc;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.JsonModels;
using StartupsBack.ViewModels;
using StartupsBack.ViewModels.ActionsResults;
using System.Text.Json;

namespace StartupsBack.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly MainDb _dbContext;
        private readonly UserControlViewModel _userControl;
        public ProfileController(ILogger<ProfileController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userControl = new UserControlViewModel(_logger, _dbContext);
        }

        //http://localhost/profile/createuser
        [HttpPost]
        public async Task<IActionResult> CreateUser()
        {
            var jsonoptions = new JsonSerializerOptions();
            jsonoptions.Converters.Add(new UserJsonModelConverter());
            var userModel = await Request.ReadFromJsonAsync<UserJsonModel>(jsonoptions);
            if (userModel == null) return BadRequest("userModel undefined");

            var createUserResult = await _userControl.CreateUserAsync(userModel);

            var answer = new UserJsonModel(createUserResult);
            var str = JsonSerializer.Serialize(answer, jsonoptions);

            return new OkObjectResult(str);
        }
        public async Task<IActionResult> Authenticate()
        {
            var jsonoptions = new JsonSerializerOptions();
            jsonoptions.Converters.Add(new UserJsonModelConverter());
            var userModel = await Request.ReadFromJsonAsync<UserJsonModel>(jsonoptions);
            if (userModel == null) return BadRequest("userModel undefined");

            var userAuthenticateResult = await _userControl.AuthenticationAsync(userModel);

            var answer = new UserJsonModel(userAuthenticateResult);
            var str = JsonSerializer.Serialize(answer, jsonoptions);

            return new OkObjectResult(str);
        }
    }
}
