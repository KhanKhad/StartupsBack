using StartupsBack.Models.DbModels;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class StartupCreateResult
    {
        public StartupCreateResultType StartupCreateResultType { get; set; } = StartupCreateResultType.Unknown;
        public StartupModel? StartupOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public StartupCreateResult()
        {

        }

        public static StartupCreateResult Success(StartupModel startup)
        {
            var res = new StartupCreateResult()
            {
                ErrorOrNull = null,
                StartupCreateResultType = StartupCreateResultType.Success,
                StartupOrNull = startup
            };
            return res;
        }
        public static StartupCreateResult AlreadyExists()
        {
            var res = new StartupCreateResult()
            {
                ErrorOrNull = null,
                StartupCreateResultType = StartupCreateResultType.AlreadyExist,
                StartupOrNull = null
            };
            return res;
        }

        public static StartupCreateResult AuthenticationFailed()
        {
            var res = new StartupCreateResult()
            {
                ErrorOrNull = null,
                StartupCreateResultType = StartupCreateResultType.AuthenticationFailed,
                StartupOrNull = null
            };
            return res;
        }

        public static StartupCreateResult UnknownError(Exception exception)
        {
            var res = new StartupCreateResult()
            {
                ErrorOrNull = exception,
                StartupCreateResultType = StartupCreateResultType.UnknownError,
                StartupOrNull = null
            };
            return res;
        }
    }

    public enum StartupCreateResultType
    {
        Unknown,
        Success,
        AlreadyExist,
        AuthenticationFailed,

        UnknownError
    }
}
