using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Elysia
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class SaveData
    {
        [JsonProperty(Order = int.MinValue)] public int Version { get; [System.Obsolete("Do not modify")] set; }
    }
}
