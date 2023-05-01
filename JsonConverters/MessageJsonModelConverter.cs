using Microsoft.AspNetCore.Hosting;
using StartupsBack.Models.JsonModels;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StartupsBack.JsonConverters
{
    public class MessageJsonModelConverter : JsonConverter<MessageJsonModel>
    {
        public override MessageJsonModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? message = null;
            string? sender = null;
            string? recipient = null;
            string? hash = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName?.ToLower())
                    {
                        case JsonConstants.MessageText when reader.TokenType == JsonTokenType.String:
                            message = reader.GetString();
                            break;
                        case JsonConstants.MessageRecipient when reader.TokenType == JsonTokenType.String:
                            recipient = reader.GetString();
                            break;
                        case JsonConstants.MessageSender when reader.TokenType == JsonTokenType.String:
                            sender = reader.GetString();
                            break;
                        case JsonConstants.MessageHash when reader.TokenType == JsonTokenType.String:
                            hash = reader.GetString();
                            break;
                    }
                }
            }
            message ??= string.Empty;
            recipient ??= string.Empty;
            sender ??= string.Empty;
            hash ??= string.Empty;

            return new MessageJsonModel()
            {
                Message = message,
                Sender = sender,
                Recipient = recipient,
                Hash = hash
            };
        }

        public override void Write(Utf8JsonWriter writer, MessageJsonModel message, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(JsonConstants.MessageId, message.Id.ToString());
            writer.WriteEndObject();
        }
    }
}
