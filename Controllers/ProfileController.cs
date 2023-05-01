﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using StartupsBack.Database;
using StartupsBack.JsonConverters;
using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using StartupsBack.Utilities;
using StartupsBack.ViewModels;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StartupsBack.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly MainDb _dbContext;
        private readonly UsersManagmentViewModel _userControl;
        private readonly string[] _permittedExtensions = { ".txt", ".png", ".jpg", ".jpeg" };
        private readonly long _fileSizeLimit;

        public ProfileController(ILogger<ProfileController> logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _fileSizeLimit = 1048576 * 3;
            _userControl = new UsersManagmentViewModel(_logger, _dbContext);
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

            var userAuthenticateResult = await _userControl.AuthenticationAsync(userModel.Name, userModel.Password);

            var answer = new UserJsonModel(userAuthenticateResult);
            var str = JsonSerializer.Serialize(answer, jsonoptions);

            return new OkObjectResult(str);
        }

        public async Task<IActionResult> CreateUserFromMultipart()
        {
            var userParseResult = await MultipartRequestHelper.GetUserModelFromMultipart(Request, ModelState, _permittedExtensions, _fileSizeLimit);

            if (userParseResult.UserOrNull == null)
            {
                if(userParseResult.UserParseResultType == ViewModels.ActionsResults.UserParseResultType.BadModel)
                {
                    var sb = new StringBuilder();
                    sb.AppendJoin(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));

                    return BadRequest(new { UserParseResult = userParseResult.UserParseResultType.ToString(), ErrorOrEmpty = sb.ToString() });
                }
                else
                    return BadRequest(new { UserParseResult = userParseResult.UserParseResultType.ToString(), ErrorOrEmpty = userParseResult.ErrorOrNull == null ? string.Empty : userParseResult.ErrorOrNull.Message });

            }

            var createUserResult = await _userControl.CreateUserAsync(userParseResult.UserOrNull);

            return Json(new { Result = createUserResult.UserCreateResultType.ToString(), Token = createUserResult.UserOrNull?.Token ?? string.Empty });
        }

        public async Task<IActionResult> AutenticateAndGetMultipart(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("name or password is empty");
            }

            var userAuthenticateResult = await _userControl.AuthenticationAsync(name, password);

            if (userAuthenticateResult.UserOrNull == null)
            {
                return BadRequest(new { Result = userAuthenticateResult.AuthenticationResultType.ToString(), ErrorOrEmpty = userAuthenticateResult.ErrorOrNull == null ? string.Empty : userAuthenticateResult.ErrorOrNull.Message });
            }

            return new MultiformActionResult(userAuthenticateResult.UserOrNull, true, true);
        }

        /*public async Task Download()
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
        }*/
    }
}
