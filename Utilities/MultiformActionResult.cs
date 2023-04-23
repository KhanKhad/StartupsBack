using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartupsBack.Models.DbModels;
using System;
using System.Threading.Tasks;

namespace StartupsBack.Utilities
{
    public class MultiformActionResult : IActionResult
    {
        private UserModel _user;
        private bool _needPec;
        private bool _myProfile;

        public MultiformActionResult(UserModel userModel, bool needPic, bool myProfile)
        {
            _myProfile = myProfile;
            _user = userModel;
            _needPec = needPic;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            string formDataBoundary = string.Format("----------{0:N}", DateTime.Now.Ticks.ToString("x"));
            context.HttpContext.Response.ContentType = "multipart/form-data; boundary=" + formDataBoundary;

            var multipartresult = await MultipartRequestHelper.UserModelToMultipart(formDataBoundary, _user, _needPec, _myProfile);
            await multipartresult.CopyToAsync(context.HttpContext.Response.Body);
        }
    }
}
