using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.JsonModels;
using StartupsBack.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

namespace StartupsBack.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly MainDb _dbContext;
        private MessagesManagmentViewModel _messagesManagment;
        public MessagesController(ILogger<MessagesController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _messagesManagment = new MessagesManagmentViewModel(logger, dbContext);
        }

        public async Task<IActionResult> SendMessage()
        {
            var jsonoptions = new JsonSerializerOptions();
            jsonoptions.Converters.Add(new MessageJsonModelConverter());
            var messageModel = await Request.ReadFromJsonAsync<MessageJsonModel>(jsonoptions);
            if (messageModel == null) return BadRequest("messageModel undefined");

            var sendMessageResult = await _messagesManagment.SendMessageAsync(messageModel.Message, messageModel.Sender, messageModel.Recipient, messageModel.Hash);

            return Json(new { Result = sendMessageResult.SendMessageResultType.ToString(), ErrorOrEmpty = sendMessageResult.ErrorOrNull == null ? string.Empty : sendMessageResult.ErrorOrNull.Message });
        }

        public async Task<IActionResult> GetMessages(string name, string hash)
        {
            var getMessagesResult = await _messagesManagment.GetMessagesAsync(name, hash);

            if(getMessagesResult.MessagesOrNull == null)
                return BadRequest(new { Result = getMessagesResult.GetMessagesResultType.ToString(), ErrorOrEmpty = getMessagesResult.ErrorOrNull == null ? string.Empty : getMessagesResult.ErrorOrNull.Message });
            
            return Json(getMessagesResult.MessagesOrNull);
        }
    }
}
