using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.IDSystem
{
    public sealed class InstanceIDJsonConverter : JsonConverter<InstanceID>
    {
        public override InstanceID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new InstanceID(new(reader.GetString()
                ?? throw new JsonException("InstanceID cannot be null.")));
        }

        public override void Write(Utf8JsonWriter writer, InstanceID value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }

        public override InstanceID ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new InstanceID(new(reader.GetString()
                ?? throw new JsonException("InstanceID property name cannot be null.")));
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, InstanceID value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.Value.ToString());
        }
    }
}
