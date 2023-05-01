using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;
using StartupsBack.ViewModels;

namespace StartupsBack.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly MainDb _dbContext;
        private readonly UserControlViewModel _userControl;

        public UsersController(ILogger<UsersController> logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
            _userControl = new UserControlViewModel(_logger, _dbContext);
        }
    }
}
