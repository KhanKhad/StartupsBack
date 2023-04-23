using StartupsBack.Models.DbModels;
using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class UserParseResult
    {
        public UserParseResultType UserParseResultType { get; set; } = UserParseResultType.Unknown;
        public UserModel? UserOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public UserParseResult()
        {

        }

        public static UserParseResult Success(UserModel user)
        {
            var res = new UserParseResult()
            {
                ErrorOrNull = null,
                UserParseResultType = UserParseResultType.Success,
                UserOrNull = user
            };
            return res;
        }

        public static UserParseResult NotMultipart()
        {
            var res = new UserParseResult()
            {
                ErrorOrNull = null,
                UserParseResultType = UserParseResultType.NotMultipart,
                UserOrNull = null
            };
            return res;
        }

        public static UserParseResult BadFields()
        {
            var res = new UserParseResult()
            {
                ErrorOrNull = null,
                UserParseResultType = UserParseResultType.BadFields,
                UserOrNull = null
            };
            return res;
        }
        public static UserParseResult BadModel()
        {
            var res = new UserParseResult()
            {
                ErrorOrNull = null,
                UserParseResultType = UserParseResultType.BadModel,
                UserOrNull = null
            };
            return res;
        }

        public static UserParseResult UnknownError(Exception exception)
        {
            var res = new UserParseResult()
            {
                ErrorOrNull = exception,
                UserParseResultType = UserParseResultType.UnknownError,
                UserOrNull = null
            };
            return res;
        }
    }

    public enum UserParseResultType
    {
        Unknown,
        Success,
        NotMultipart,
        BadFields,
        BadModel,
        UnknownError
    }
}
