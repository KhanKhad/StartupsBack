using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;

namespace StartupsBack.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly MainDb _dbContext;

        public MessagesController(ILogger<MessagesController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

    }
}
