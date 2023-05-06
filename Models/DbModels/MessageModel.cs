using Newtonsoft.Json;
using StartupsBack.JsonConverters;
using System;
using System.ComponentModel.DataAnnotations;

namespace StartupsBack.Models.DbModels
{
    public class MessageModel
    {
        [Key, JsonProperty(JsonConstants.MessageId)]
        public int Id { get; set; }
        [JsonProperty(JsonConstants.MessageText)]
        public string Message { get; set; } = string.Empty;

        [JsonProperty(JsonConstants.MessageSenderId)]
        public int SenderForeignKey { get; set; }

        [JsonIgnore]
        public UserModel? Sender { get; set; }

        [JsonProperty(JsonConstants.MessageRecipientId)]
        public int RecipientForeignKey { get; set; }

        [JsonIgnore]
        public UserModel? Recipient { get; set; }

        [JsonProperty(JsonConstants.MessageSended)]
        public DateTime MessageSended { get; set; }
        [JsonProperty(JsonConstants.MessageReaded)]
        public DateTime MessageReaded { get; set; }

        [JsonProperty(JsonConstants.MessageIsGetted)]
        public bool IsGetted { get; set; }
        [JsonProperty(JsonConstants.MessageIsReaded)]
        public bool IsReaded { get; set; }
        [JsonProperty(JsonConstants.MessageDelta)]
        public int Delta { get; set; }
    }
}
