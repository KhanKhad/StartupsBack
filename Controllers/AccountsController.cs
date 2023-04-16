using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;

namespace StartupsBack.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly MainDb _dbContext;

        public AccountsController(ILogger<AccountsController> logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
        }

    }
}
