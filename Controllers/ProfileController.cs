using Microsoft.AspNetCore.Mvc;
using StartupsBack.Database;
using StartupsBack.ViewModels;
using StartupsBack.ViewModels.ActionsResults;

namespace StartupsBack.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly MainDb _dbContext;
        UserControlViewModel _userControl;
        public ProfileController(ILogger<ProfileController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userControl = new UserControlViewModel(_logger, dbContext);
        }

        //http://localhost/profile/createuser?name=to2m&pass=dsf
        public async Task<IActionResult> CreateUser(string name, string pass)
        {
            var res = await _userControl.CreateUserAsync(name, pass);

            switch (res.UserCreateResultType)
            {
                case UserCreateResultType.Success:
                    return new JsonResult(new { Result = UserCreateResultType.Success.ToString() });

                case UserCreateResultType.AlreadyExist:
                    return new JsonResult(new { Result = UserCreateResultType.AlreadyExist.ToString() });

                case UserCreateResultType.UnknownError:
                    return new JsonResult(new { Result = UserCreateResultType.UnknownError.ToString(), Error = res.ErrorOrNull });
                default:
                    break;
            }

            return new ContentResult() { Content = "lala" };
        }
    }
}
