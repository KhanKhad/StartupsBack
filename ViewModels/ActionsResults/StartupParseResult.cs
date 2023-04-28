using StartupsBack.Models.DbModels;
using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class StartupParseResult
    {
        public StartupParseResultType StartupParseResultType { get; set; } = StartupParseResultType.Unknown;
        public StartupModel? StartupOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }
        public string AuthorNameOrEmpty { get; set; } = string.Empty;
        public string StartupHash { get; set; } = string.Empty;


        public StartupParseResult()
        {

        }

        public static StartupParseResult Success(StartupModel startup, string author, string hash)
        {
            var res = new StartupParseResult()
            {
                ErrorOrNull = null,
                StartupParseResultType = StartupParseResultType.Success,
                StartupOrNull = startup,
                AuthorNameOrEmpty = author,
                StartupHash = hash
            };
            return res;
        }

        public static StartupParseResult NotMultipart()
        {
            var res = new StartupParseResult()
            {
                ErrorOrNull = null,
                StartupParseResultType = StartupParseResultType.NotMultipart,
                StartupOrNull = null
            };
            return res;
        }

        public static StartupParseResult BadFields()
        {
            var res = new StartupParseResult()
            {
                ErrorOrNull = null,
                StartupParseResultType = StartupParseResultType.BadFields,
                StartupOrNull = null
            };
            return res;
        }
        public static StartupParseResult BadModel()
        {
            var res = new StartupParseResult()
            {
                ErrorOrNull = null,
                StartupParseResultType = StartupParseResultType.BadModel,
                StartupOrNull = null
            };
            return res;
        }

        public static StartupParseResult UnknownError(Exception exception)
        {
            var res = new StartupParseResult()
            {
                ErrorOrNull = exception,
                StartupParseResultType = StartupParseResultType.UnknownError,
                StartupOrNull = null
            };
            return res;
        }
    }

    public enum StartupParseResultType
    {
        Unknown,
        Success,
        NotMultipart,
        BadFields,
        BadModel,
        UnknownError
    }
}
