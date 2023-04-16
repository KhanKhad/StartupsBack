using StartupsBack.Models.JsonModels;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StartupsBack.JsonConverters
{
    public class UserJsonModelConverter : JsonConverter<UserJsonModel>
    {
        public override UserJsonModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? name = null;
            string? pass = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Name" or "name" when reader.TokenType == JsonTokenType.String:
                            name = reader.GetString();
                            break;
                        case "Password" or "password" when reader.TokenType == JsonTokenType.String:
                            pass = reader.GetString();
                            break;
                    }
                }
            }
            name ??= string.Empty;
            pass ??= string.Empty;

            return new UserJsonModel()
            {
                Name = name,
                Password = pass
            };
        }

        public override void Write(Utf8JsonWriter writer, UserJsonModel user, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("UserCreateResult", user.UserCreateResult.ToString());
            writer.WriteString("AuthenticationResult", user.AuthenticationResult.ToString());
            writer.WriteString("Token", user.Token);
            writer.WriteString("ErrorOrEmpty", user.ErrorOrEmpty);

            writer.WriteEndObject();
        }
    }
}
