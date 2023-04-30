using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartupsBack.Models.DbModels;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace StartupsBack.Utilities
{
    public class MultiformActionResult : IActionResult
    {
        private UserModel? _userModel;
        private StartupModel? _startupModel;

        private bool _needFullInfo;
        private bool _isMine;

        private MultiformType _multiformType;

        public MultiformActionResult(UserModel userModel, bool isMine, bool needFull)
        {
            _isMine = isMine;
            _needFullInfo = needFull;
            _userModel = userModel;
            _multiformType = MultiformType.UserModelMultiform;
        }

        public MultiformActionResult(StartupModel startupModel, bool isMine, bool needFull)
        {
            _isMine = isMine;
            _needFullInfo = needFull;
            _startupModel = startupModel;
            _multiformType = MultiformType.StartupModelMultiform;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            string formDataBoundary = string.Format("----------{0:N}", DateTime.Now.Ticks.ToString("x"));
            context.HttpContext.Response.ContentType = "multipart/form-data; boundary=" + formDataBoundary;
            
            var multipartFormDataresult = _multiformType switch
            {
                MultiformType.StartupModelMultiform => await MultipartRequestHelper.StartupModelToMultipart(formDataBoundary, _startupModel, _isMine, _needFullInfo),
                MultiformType.UserModelMultiform => await MultipartRequestHelper.UserModelToMultipart(formDataBoundary, _userModel, _isMine, _needFullInfo),

                _ => await MultipartRequestHelper.UserModelToMultipart(formDataBoundary, _userModel, _isMine, _needFullInfo),
            };

            await multipartFormDataresult.CopyToAsync(context.HttpContext.Response.Body);
        }
    }

    public enum MultiformType
    {
        UserModelMultiform,
        StartupModelMultiform
    }
}
