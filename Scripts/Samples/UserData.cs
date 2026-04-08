#if false

using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Elysia
{
    [SaveData("user", 3)]
    public partial class UserData : SaveData
    {
        [JsonProperty] public int Gold { get; private set; }

        public void AddGold(int value)
        {
            Gold += value;
        }
    }

    public partial class UserData
    {
        [SaveData("user", 2)]
        [System.Obsolete]
        public class V2 : SaveData, IMigrator<UserData>
        {
            [JsonProperty] public int Gold { get; set; }
            [JsonProperty] public string UserName { get; set; } = "You-Know-Who";

            public UserData Migrate()
            {
                return new UserData
                {
                    Gold = Gold
                };
            }
        }

        [SaveData("user", 1)]
        [System.Obsolete]
        public class V1 : SaveData, IMigrator<V2>
        {
            [JsonProperty] public float Gold { get; set; }
            [JsonProperty] public string UserName { get; set; } = "You-Know-Who";

            public V2 Migrate()
            {
                return new V2
                {
                    Gold = (int)Gold,
                    UserName = UserName
                };
            }
        }
    }
}

#endif
