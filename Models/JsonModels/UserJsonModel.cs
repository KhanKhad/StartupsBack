using StartupsBack.ViewModels.ActionsResults;
using System;

namespace StartupsBack.Models.JsonModels
{
    public class UserJsonModel
    {
        public UserJsonModel() { }

        #region RequestProperties
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public byte[] ProfilePic { get; set; } = Array.Empty<byte>();
        #endregion

        public UserJsonModel(AuthenticationResult authenticationResult)
        {
            AuthenticationResult = authenticationResult.AuthenticationResultType;
            Token = authenticationResult.UserOrNull?.Token ?? string.Empty;
            ErrorOrEmpty = authenticationResult.ErrorOrNull?.Message ?? string.Empty;
        }
        public UserJsonModel(UserCreateResult userCreateResult)
        {
            UserCreateResult = userCreateResult.UserCreateResultType;
            ErrorOrEmpty = userCreateResult.ErrorOrNull?.Message ?? string.Empty;
        }

        #region ResponseProperties
        public string Token { get; set; } = string.Empty;
        public UserCreateResultType UserCreateResult { get; set; }
        public AuthenticationResultType AuthenticationResult { get; set; }
        public string ErrorOrEmpty { get; set; } = string.Empty;
        #endregion
    }
}
