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
        public async Task<IActionResult> CreateStartupVer2()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            var startupModel = new StartupModel();
            string authorToken = string.Empty;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType), _fileSizeLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        if (contentDisposition?.Name == "name")
                        {
                            var val = await section.ReadAsStringAsync();
                            startupModel.Name = val;
                        }
                        else if (contentDisposition?.Name == "authortoken")
                        {
                            var val = await section.ReadAsStringAsync();
                            authorToken = val;
                        }
                        else if (contentDisposition?.Name == "description")
                        {
                            var val = await section.ReadAsStringAsync();
                            startupModel.Description = val;
                        }
                        else
                        {
                            ModelState.AddModelError("File",
                             $"The request couldn't be processed (Error 2).");
                        }
                    }
                    else
                    {
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        startupModel.Picture = streamedFileContent;
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            if (false && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            startupModel.StartupPublished = DateTime.UtcNow;
            var createStartupResult = await _startupsManager.CreateStartupAsync(startupModel, authorToken);

            return Json(new { Result = createStartupResult.StartupCreateResultType });            
        }
    }
}
