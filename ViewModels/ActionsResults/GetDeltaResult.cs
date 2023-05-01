using StartupsBack.Models.DbModels;
using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class GetDeltaResult
    {
        public GetDeltaResultType GetDeltaResultType { get; set; } = GetDeltaResultType.Unknown;
        public int Delta { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public GetDeltaResult()
        {

        }

        public static GetDeltaResult Success(int delta)
        {
            var res = new GetDeltaResult()
            {
                ErrorOrNull = null,
                GetDeltaResultType = GetDeltaResultType.Success,
                Delta = delta
            };
            return res;
        }

        public static GetDeltaResult UserNotFound()
        {
            var res = new GetDeltaResult()
            {
                ErrorOrNull = null,
                GetDeltaResultType = GetDeltaResultType.UserNotFound,
                Delta = -1
            };
            return res;
        }

        public static GetDeltaResult UnknownError(Exception exception)
        {
            var res = new GetDeltaResult()
            {
                ErrorOrNull = exception,
                GetDeltaResultType = GetDeltaResultType.UnknownError,
                Delta = -1
            };
            return res;
        }
    }

    public enum GetDeltaResultType
    {
        Unknown,
        Success,
        UserNotFound,

        UnknownError
    }
}
