using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AshborneGame._Core.Data.IDSystem
{
    public sealed class DefinitionIDJsonConverter : JsonConverter<DefinitionID>
    {
        public override DefinitionID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new DefinitionID(reader.GetString()
                ?? throw new JsonException("DefinitionID cannot be null."));
        }

        public override void Write(Utf8JsonWriter writer, DefinitionID value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }

        public override DefinitionID ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new DefinitionID(reader.GetString()
                ?? throw new JsonException("DefinitionID property name cannot be null."));
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, DefinitionID value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.Value);
        }
    }
}