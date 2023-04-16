using StartupsBack.Models.DbModels;
using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class UserCreateResult
    {
        public UserCreateResultType UserCreateResultType { get; set; } = UserCreateResultType.Unknown;
        public UserModel? UserOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public UserCreateResult() 
        {

        }

        public static UserCreateResult Success(UserModel user) 
        {
            var res = new UserCreateResult()
            {
                ErrorOrNull = null,
                UserCreateResultType = UserCreateResultType.Success,
                UserOrNull = user
            };
            return res;
        }
        public static UserCreateResult AlreadyExists()
        {
            var res = new UserCreateResult()
            {
                ErrorOrNull = null,
                UserCreateResultType = UserCreateResultType.AlreadyExist,
                UserOrNull = null
            };
            return res;
        }
        public static UserCreateResult UnknownError(Exception exception)
        {
            var res = new UserCreateResult()
            {
                ErrorOrNull = exception,
                UserCreateResultType = UserCreateResultType.UnknownError,
                UserOrNull = null
            };
            return res;
        }
    }

    public enum UserCreateResultType
    {
        Unknown,
        Success,
        AlreadyExist,

        UnknownError
    }
}
