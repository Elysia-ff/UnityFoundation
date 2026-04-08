using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Elysia
{
    public class JsonNotFoundVersionException : JsonException
    {
        public JsonNotFoundVersionException()
        {
        }

        public JsonNotFoundVersionException(string message)
            : base(message)
        {
        }

        public JsonNotFoundVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public JsonNotFoundVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
