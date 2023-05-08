using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class AcceptUserResult
    {
        public AcceptUserResultType AcceptUserResultType { get; set; } = AcceptUserResultType.Unknown;
        public Exception? ErrorOrNull { get; set; }

        public AcceptUserResult()
        {

        }

        public static AcceptUserResult SuccessJoined()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.SuccessJoined,
            };
            return res;
        }

        public static AcceptUserResult AlreadyJoined()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.AlreadyJoined
            };
            return res;
        }
        public static AcceptUserResult UserDontSendRequest()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.UserDontSendRequest
            };
            return res;
        }

        public static AcceptUserResult AuthenticationFailed()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.AuthenticationFailed
            };
            return res;
        }

        public static AcceptUserResult NotAuthor()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.NotAuthor
            };
            return res;
        }

        public static AcceptUserResult AuthorNotFound()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.AuthorNotFound
            };
            return res;
        }
        public static AcceptUserResult UserNotFound()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.UserNotFound
            };
            return res;
        }
        public static AcceptUserResult StartupNotFound()
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = null,
                AcceptUserResultType = AcceptUserResultType.StartupNotFound
            };
            return res;
        }

        public static AcceptUserResult UnknownError(Exception exception)
        {
            var res = new AcceptUserResult()
            {
                ErrorOrNull = exception,
                AcceptUserResultType = AcceptUserResultType.UnknownError
            };
            return res;
        }
    }

    public enum AcceptUserResultType
    {
        Unknown,
        SuccessJoined,
        AlreadyJoined,
        AuthenticationFailed,
        UserNotFound,
        StartupNotFound,
        AuthorNotFound,
        UserDontSendRequest,
        NotAuthor,
        UnknownError
    }
}
