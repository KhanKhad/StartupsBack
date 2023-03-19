using StartupsBack.Models.DbModels;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class AuthenticationResult
    {
        public AuthenticationResultType AuthenticationResultType { get; set; } = AuthenticationResultType.Unknown;
        public UserModel? UserOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public AuthenticationResult()
        {

        }

        public static AuthenticationResult Success(UserModel user)
        {
            var res = new AuthenticationResult()
            {
                ErrorOrNull = null,
                AuthenticationResultType = AuthenticationResultType.Success,
                UserOrNull = user
            };
            return res;
        }

        public static AuthenticationResult WrongPassword()
        {
            var res = new AuthenticationResult()
            {
                ErrorOrNull = null,
                AuthenticationResultType = AuthenticationResultType.WrongPassword,
                UserOrNull = null
            };
            return res;
        }

        public static AuthenticationResult WrongLogin()
        {
            var res = new AuthenticationResult()
            {
                ErrorOrNull = null,
                AuthenticationResultType = AuthenticationResultType.WrongLogin,
                UserOrNull = null
            };
            return res;
        }

        public static AuthenticationResult UnknownError(Exception exception)
        {
            var res = new AuthenticationResult()
            {
                ErrorOrNull = exception,
                AuthenticationResultType = AuthenticationResultType.UnknownError,
                UserOrNull = null
            };
            return res;
        }
    }
    public enum AuthenticationResultType
    {
        Unknown,
        Success,
        WrongPassword,
        WrongLogin,

        UnknownError
    }
}
