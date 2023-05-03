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

        [JsonProperty(JsonConstants.MessageSender)]
        public int SenderForeignKey { get; set; }

        [JsonIgnore]
        public UserModel? Sender { get; set; }

        [JsonProperty(JsonConstants.MessageRecipient)]
        public int RecipientForeignKey { get; set; }

        [JsonIgnore]
        public UserModel? Recipient { get; set; }

        public DateTime MessageSended { get; set; }
        public DateTime Messagereaded { get; set; }

        public bool IsGetted { get; set; }
        public bool IsReaded { get; set; }
    }
}
