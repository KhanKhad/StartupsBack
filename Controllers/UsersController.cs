using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;
using StartupsBack.Utilities;
using StartupsBack.ViewModels;
using System.Threading.Tasks;

namespace StartupsBack.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly MainDb _dbContext;
        private readonly UsersManagmentViewModel _userControl;

        public UsersController(ILogger<UsersController> logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
            _userControl = new UsersManagmentViewModel(_logger, _dbContext);
        }


        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userControl.GetUserById(id);
            if(user == null) return NotFound();

            return new MultiformActionResult(user, false, true);
        }
    }
}
