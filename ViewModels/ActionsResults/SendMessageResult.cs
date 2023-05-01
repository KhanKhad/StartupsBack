using StartupsBack.Models.DbModels;
using System;

namespace StartupsBack.ViewModels.ActionsResults
{
    public class SendMessageResult
    {
        public SendMessageResultType SendMessageResultType { get; set; } = SendMessageResultType.Unknown;
        public MessageModel? MessageOrNull { get; set; }
        public Exception? ErrorOrNull { get; set; }

        public SendMessageResult()
        {

        }

        public static SendMessageResult Success(MessageModel message)
        {
            var res = new SendMessageResult()
            {
                ErrorOrNull = null,
                SendMessageResultType = SendMessageResultType.Success,
                MessageOrNull = message
            };
            return res;
        }

        public static SendMessageResult AuthorNotFound()
        {
            var res = new SendMessageResult()
            {
                ErrorOrNull = null,
                SendMessageResultType = SendMessageResultType.AuthorNotFound,
                MessageOrNull = null
            };
            return res;
        }

        public static SendMessageResult RecipientNotFound()
        {
            var res = new SendMessageResult()
            {
                ErrorOrNull = null,
                SendMessageResultType = SendMessageResultType.RecipientNotFound,
                MessageOrNull = null
            };
            return res;
        }

        public static SendMessageResult AuthenticationFailed()
        {
            var res = new SendMessageResult()
            {
                ErrorOrNull = null,
                SendMessageResultType = SendMessageResultType.AuthenticationFailed,
                MessageOrNull = null
            };
            return res;
        }

        public static SendMessageResult UnknownError(Exception exception)
        {
            var res = new SendMessageResult()
            {
                ErrorOrNull = exception,
                SendMessageResultType = SendMessageResultType.UnknownError,
                MessageOrNull = null
            };
            return res;
        }
    }

    public enum SendMessageResultType
    {
        Unknown,
        Success,
        AuthenticationFailed,
        AuthorNotFound,
        RecipientNotFound,

        UnknownError
    }
}
