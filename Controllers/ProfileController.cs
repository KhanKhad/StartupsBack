using Microsoft.AspNetCore.Mvc;
using StartupsBack.Database;
using StartupsBack.ViewModels;

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

        public async Task<IActionResult> Index()
        {
            var res = await _userControl.CreateUserAsync("lalka", "loks");

            if (res.Item1 == UserCreateResult.Success)
            {
                return Json(res.Item2);
            }
            else return new ContentResult() { Content = "lala" };

            /*var users = _dbContext.UsersDB.Include(x => x.PublishedStartups).Include(x => x.History).ToList();
            var startups = _dbContext.StartupsDB.ToList();
            var t = users.FirstOrDefault(i => i.Id == 1);

            return Json(users);*/
        }
    }
}
