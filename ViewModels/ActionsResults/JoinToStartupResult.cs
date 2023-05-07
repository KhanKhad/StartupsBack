using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class JoinToStartupResult
    {
        public JoinToStartupResultType JoinToStartupResultType { get; set; } = JoinToStartupResultType.Unknown;
        public Exception? ErrorOrNull { get; set; }

        public JoinToStartupResult()
        {

        }

        public static JoinToStartupResult RequestSended()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.RequestSended,
            };
            return res;
        }
        public static JoinToStartupResult AlreadyJoined()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.AlreadyJoined
            };
            return res;
        }
        public static JoinToStartupResult WaitForAnswer()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.WaitForAnswer
            };
            return res;
        }
        public static JoinToStartupResult AccsessDenied()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.AccsessDenied
            };
            return res;
        }

        public static JoinToStartupResult AuthenticationFailed()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.AuthenticationFailed
            };
            return res;
        }
        public static JoinToStartupResult UserNotFound()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.UserNotFound
            };
            return res;
        }
        public static JoinToStartupResult StartupNotFound()
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = null,
                JoinToStartupResultType = JoinToStartupResultType.StartupNotFound
            };
            return res;
        }

        public static JoinToStartupResult UnknownError(Exception exception)
        {
            var res = new JoinToStartupResult()
            {
                ErrorOrNull = exception,
                JoinToStartupResultType = JoinToStartupResultType.UnknownError
            };
            return res;
        }
    }

    public enum JoinToStartupResultType
    {
        Unknown,
        RequestSended,
        AlreadyJoined,
        AuthenticationFailed,
        AccsessDenied,
        UserNotFound,
        StartupNotFound,
        WaitForAnswer,
        UnknownError
    }
}
