using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class RejectUserResult
    {
        public RejectUserResultType RejectUserResultType { get; set; } = RejectUserResultType.Unknown;
        public Exception? ErrorOrNull { get; set; }

        public RejectUserResult()
        {

        }

        public static RejectUserResult SuccessDenied()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.SuccessDenied,
            };
            return res;
        }
        public static RejectUserResult AlreadyDenied()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.AlreadyDenied
            };
            return res;
        }

        public static RejectUserResult AuthenticationFailed()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.AuthenticationFailed
            };
            return res;
        }
        public static RejectUserResult UserNotFound()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.UserNotFound
            };
            return res;
        }
        public static RejectUserResult StartupNotFound()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.StartupNotFound
            };
            return res;
        }

        public static RejectUserResult UnknownError(Exception exception)
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = exception,
                RejectUserResultType = RejectUserResultType.UnknownError
            };
            return res;
        }
        public static RejectUserResult NotAuthor()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.NotAuthor
            };
            return res;
        }

        public static RejectUserResult AuthorNotFound()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.AuthorNotFound
            };
            return res;
        }
        public static RejectUserResult UserDontSendRequest()
        {
            var res = new RejectUserResult()
            {
                ErrorOrNull = null,
                RejectUserResultType = RejectUserResultType.UserDontSendRequest
            };
            return res;
        }
    }

    public enum RejectUserResultType
    {
        Unknown,
        SuccessDenied,
        AlreadyDenied,
        AuthenticationFailed,
        UserNotFound,
        StartupNotFound,
        AuthorNotFound,
        UserDontSendRequest,
        NotAuthor,
        UnknownError
    }
}
