using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class AudioSourceExtensions
    {
        public static void PlayOneShot(this AudioSource audioSource, string key, float volumeScale = 1f)
        {
            AudioClip clip = Game.Audio.GetClip(key);

            audioSource.PlayOneShot(clip, volumeScale);
        }
    }
}
