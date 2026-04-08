using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Elysia
{
    public class StringIDJsonConverter : JsonConverter<StringID>
    {
        public override void WriteJson(JsonWriter writer, StringID value, JsonSerializer serializer)
        {
            writer.WriteValue((string)value);
        }

        public override StringID ReadJson(JsonReader reader, Type objectType, StringID existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            string value = reader.Value as string;

            return new StringID(value);
        }
    }
}
