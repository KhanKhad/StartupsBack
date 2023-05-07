using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class JoinConfurmResult
    {
        public JoinConfurmResultType JoinConfurmResultType { get; set; } = JoinConfurmResultType.Unknown;
        public Exception? ErrorOrNull { get; set; }

        public JoinConfurmResult()
        {

        }

        public static JoinConfurmResult RequestSended()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.RequestSended,
            };
            return res;
        }
        public static JoinConfurmResult AlreadyJoined()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.AlreadyJoined
            };
            return res;
        }
        public static JoinConfurmResult WaitForAnswer()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.WaitForAnswer
            };
            return res;
        }
        public static JoinConfurmResult AccsessDenied()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.AccsessDenied
            };
            return res;
        }

        public static JoinConfurmResult AuthenticationFailed()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.AuthenticationFailed
            };
            return res;
        }
        public static JoinConfurmResult UserNotFound()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.UserNotFound
            };
            return res;
        }
        public static JoinConfurmResult StartupNotFound()
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = null,
                JoinConfurmResultType = JoinConfurmResultType.StartupNotFound
            };
            return res;
        }

        public static JoinConfurmResult UnknownError(Exception exception)
        {
            var res = new JoinConfurmResult()
            {
                ErrorOrNull = exception,
                JoinConfurmResultType = JoinConfurmResultType.UnknownError
            };
            return res;
        }
    }

    public enum JoinConfurmResultType
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
