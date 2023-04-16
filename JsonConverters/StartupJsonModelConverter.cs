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
            string? authorToken = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName?.ToLower())
                    {
                        case "name" when reader.TokenType == JsonTokenType.String:
                            name = reader.GetString();
                            break;
                        case "authortoken" when reader.TokenType == JsonTokenType.String:
                            authorToken = reader.GetString();
                            break;
                        case "description" when reader.TokenType == JsonTokenType.String:
                            description = reader.GetString();
                            break;
                    }
                }
            }
            name ??= string.Empty;
            authorToken ??= string.Empty;
            description ??= string.Empty;

            return new StartupJsonModel()
            {
                Name = name,
                Description = description,
                AuthorToken = authorToken,
            };
        }

        public override void Write(Utf8JsonWriter writer, StartupJsonModel startup, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("StartupCreateResult", startup.StartupCreateResult.ToString());
            writer.WriteString("StartupId", startup.StartupId.ToString());
            writer.WriteString("ErrorOrEmpty", startup.ErrorOrEmpty);

            writer.WriteEndObject();
        }
    }
}
