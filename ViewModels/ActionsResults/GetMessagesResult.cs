using StartupsBack.Models.DbModels;
using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class GetMessagesResult
    {
        public GetMessagesResultType GetMessagesResultType { get; set; } = GetMessagesResultType.Unknown;
        public MessageModel[]? MessagesOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public GetMessagesResult()
        {

        }

        public static GetMessagesResult Success(MessageModel[] messages)
        {
            var res = new GetMessagesResult()
            {
                ErrorOrNull = null,
                GetMessagesResultType = GetMessagesResultType.Success,
                MessagesOrNull = messages
            };
            return res;
        }

        public static GetMessagesResult UserNotFound()
        {
            var res = new GetMessagesResult()
            {
                ErrorOrNull = null,
                GetMessagesResultType = GetMessagesResultType.UserNotFound,
                MessagesOrNull = null
            };
            return res;
        }

        public static GetMessagesResult AuthenticationFailed()
        {
            var res = new GetMessagesResult()
            {
                ErrorOrNull = null,
                GetMessagesResultType = GetMessagesResultType.AuthenticationFailed,
                MessagesOrNull = null
            };
            return res;
        }

        public static GetMessagesResult UnknownError(Exception exception)
        {
            var res = new GetMessagesResult()
            {
                ErrorOrNull = exception,
                GetMessagesResultType = GetMessagesResultType.UnknownError,
                MessagesOrNull = null
            };
            return res;
        }
    }

    public enum GetMessagesResultType
    {
        Unknown,
        Success,
        AuthenticationFailed,
        UserNotFound,

        UnknownError
    }
}
