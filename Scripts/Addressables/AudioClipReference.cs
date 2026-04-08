using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [Serializable]
    public class AudioClipReference : AssetReferenceT<AudioClip>
    {
        public AudioClipReference(string guid)
            : base(guid)
        {
        }
    }
}
