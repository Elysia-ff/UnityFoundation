using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [Serializable]
    public class AudioClipVolumeReference
    {
        [SerializeField] private AudioClipReference _clip;
        public AudioClipReference Clip => _clip;

        [SerializeField] private float _volume = 1f;
        public float Volume => _volume;
    }
}
