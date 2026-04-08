using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class AudioSourceExtensions
    {
        public static void PlayOneShot(this AudioSource audioSource, StringID key, float volumeScale = 1f)
        {
            if (key == StringID.NULL)
            {
                Debug.Log($"sfx play on {audioSource.gameObject.name}");
                return;
            }

            AudioClip clip = App.ResourceHolder.GetAudioClip(key);
            audioSource.PlayOneShot(clip, volumeScale);
        }
    }
}
