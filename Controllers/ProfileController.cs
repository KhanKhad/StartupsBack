using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.JsonModels;
using StartupsBack.Utilities;
using StartupsBack.ViewModels;
using StartupsBack.ViewModels.ActionsResults;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text.Json;

namespace StartupsBack.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly MainDb _dbContext;
        private readonly UserControlViewModel _userControl;
        private readonly string[] _permittedExtensions = { ".txt", ".png", ".jpg" };
        private readonly long _fileSizeLimit;

        public ProfileController(ILogger<ProfileController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _fileSizeLimit = long.MaxValue;
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

        public async Task<IActionResult> UploadPhysical()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

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
                        ModelState.AddModelError("File",
                            $"The request couldn't be processed (Error 2).");
                        // Log error

                        //return BadRequest(ModelState);
                    }
                    else
                    {
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }
                        var targetFilePath = Path.GetTempFileName();

                        using (var targetStream = System.IO.File.Create(
                            Path.Combine(targetFilePath, trustedFileNameForFileStorage)))
                        {
                            await targetStream.WriteAsync(streamedFileContent);

                            _logger.LogInformation(
                                "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                trustedFileNameForDisplay, targetFilePath,
                                trustedFileNameForFileStorage);
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return Ok();
        }


        public async Task resp()
        {
            HttpContext.Response.Clear();
            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

            string formDataBoundary = String.Format("----------{0:N}", DateTime.Now.Ticks.ToString("x"));
            HttpContext.Response.ContentType = "multipart/form-data; boundary=" + formDataBoundary;

            string[] files = new string[] { "file.txt", "file.txt" };
            Stream memStream = new MemoryStream();
            var boundarybytes = System.Text.Encoding.UTF8.GetBytes("--" + formDataBoundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + formDataBoundary + "--");

            foreach (var FileName in files)
            {
                WriteFilePart(FileName, boundarybytes, memStream);
                boundarybytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + formDataBoundary + "\r\n");
            }

            await memStream.WriteAsync(endBoundaryBytes, 0, endBoundaryBytes.Length);
            
            memStream.Position = 0;

            await memStream.CopyToAsync(HttpContext.Response.Body);
        }
        private void WriteFilePart(string FileName, byte[] boundarybytes, Stream memStream)
        {
            string Tosend = "";
            Tosend += $"Content-Type: text/plain{Environment.NewLine}";
            Tosend += $"Content-Location: {FileName}{Environment.NewLine}";
            Tosend += $"Content-Disposition: attachment; filename=\"{FileName}\"{Environment.NewLine}";
            Tosend += $"Content-ID: {FileName}{Environment.NewLine}";
            Tosend += Environment.NewLine;

            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            var headerbytes = System.Text.Encoding.UTF8.GetBytes(Tosend);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var filebyte = System.IO.File.ReadAllBytes(Path.Combine("files", FileName));

            memStream.Write(filebyte, 0, filebyte.Length);

        }

        public async Task send()
        {
            string formDataBoundary = String.Format("----------{0:N}", DateTime.Now.Ticks.ToString("x"));
            HttpContext.Response.ContentType = "multipart/form-data; boundary=" + formDataBoundary;

            var mp = ProcessRequestM(formDataBoundary);
            await mp.CopyToAsync(HttpContext.Response.Body);
        }

        public MultipartFormDataContent ProcessRequestM(string formDataBoundary)
        {
            Stream fileStream1 = System.IO.File.OpenRead(Path.Combine("files", "file.txt"));
            Stream fileStream2 = System.IO.File.OpenRead(Path.Combine("files", "file.txt"));

            var file_bytes = System.IO.File.ReadAllBytes(Path.Combine("files", "file.txt"));

            var formData = new MultipartFormDataContent(formDataBoundary);
            formData.Add(new StringContent("Ahalay"), "username");
            formData.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "profile_pic", "free.png");
            return formData;
        }
    }
}
