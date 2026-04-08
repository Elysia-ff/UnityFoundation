using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Elysia
{
    public class JsonEmptyException : JsonException
    {
        public JsonEmptyException()
        {
        }

        public JsonEmptyException(string message)
            : base(message)
        {
        }

        public JsonEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public JsonEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
