using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class AudioSourceExtensions
    {
        public static void PlayOneShot(this AudioSource audioSource, string key, float volumeScale = 1f)
        {
            Game.Audio.GetClipAsync(key, clip =>
            {
                audioSource.PlayOneShot(clip, volumeScale);
            });
        }
    }
}
