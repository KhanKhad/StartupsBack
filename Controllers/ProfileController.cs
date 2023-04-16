using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using StartupsBack.Utilities;
using StartupsBack.ViewModels;
using System.Text.Json;

namespace StartupsBack.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly MainDb _dbContext;
        private readonly UserControlViewModel _userControl;
        private readonly string[] _permittedExtensions = { ".txt", ".png", ".jpg", ".jpeg" };
        private readonly long _fileSizeLimit;

        public ProfileController(ILogger<ProfileController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _fileSizeLimit = 64000;
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

        public async Task<IActionResult> Upload()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            var userModel = new UserModel();

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
                        if(contentDisposition?.Name == "username")
                        {
                            var val = await section.ReadAsStringAsync();
                            userModel.Name = val;
                        }
                        else if(contentDisposition?.Name == "password")
                        {
                            var val = await section.ReadAsStringAsync();
                            userModel.PasswordHash = await UserControlViewModel.GetHashAsync(val);
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

                        userModel.ProfilePic = streamedFileContent;
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            if (false && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createUserResult = await _userControl.CreateUserAsync(userModel);

            return Json(new { Result = createUserResult.UserCreateResultType, Token = createUserResult.UserOrNull?.Token ?? string.Empty });
        }

        public async Task Download()
        {
            var res = await _userControl.GetUserAsync(1);
            string formDataBoundary = String.Format("----------{0:N}", DateTime.Now.Ticks.ToString("x"));
            HttpContext.Response.ContentType = "multipart/form-data; boundary=" + formDataBoundary;

            var mp = GetMultipart(formDataBoundary, res);
            await mp.CopyToAsync(HttpContext.Response.Body);
        }

        public MultipartFormDataContent GetMultipart(string formDataBoundary, UserModel user)
        {
            var file_bytes = user.ProfilePic;

            var formData = new MultipartFormDataContent(formDataBoundary);
            formData.Add(new StringContent(user.Name), "username");
            formData.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "profile_pic", "free.png");
            return formData;
        }
    }
}
