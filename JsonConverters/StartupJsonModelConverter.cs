using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StartupsBack.JsonConverters
{
    public class StartupJsonModelConverter : JsonConverter<StartupJsonModel>
    {
        public override StartupJsonModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? name = null;
            string? description = null;
            string? authorName = null;
            string? hash = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName?.ToLower())
                    {
                        case JsonConstants.StartupName when reader.TokenType == JsonTokenType.String:
                            name = reader.GetString();
                            break;
                        case JsonConstants.StartupAuthorName when reader.TokenType == JsonTokenType.String:
                            authorName = reader.GetString();
                            break;
                        case JsonConstants.StartupDescription when reader.TokenType == JsonTokenType.String:
                            description = reader.GetString();
                            break;
                        case JsonConstants.StartupHash when reader.TokenType == JsonTokenType.String:
                            hash = reader.GetString();
                            break;
                    }
                }
            }
            name ??= string.Empty;
            authorName ??= string.Empty;
            description ??= string.Empty;
            hash ??= string.Empty;

            return new StartupJsonModel()
            {
                Name = name,
                Description = description,
                AuthorName = authorName,
                Hash = hash
            };
        }

        public override void Write(Utf8JsonWriter writer, StartupJsonModel startup, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("StartupCreateResult", startup.StartupCreateResult.ToString());
            writer.WriteString(JsonConstants.StartupId, startup.StartupId.ToString());
            writer.WriteString("ErrorOrEmpty", startup.ErrorOrEmpty);

            writer.WriteEndObject();
        }
    }
}
