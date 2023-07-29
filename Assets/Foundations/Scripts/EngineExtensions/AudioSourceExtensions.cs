using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Elysia
{
    public static class AudioSourceExtensions
    {
        public static void PlayOneShot(this AudioSource audioSource, string key, float volumeScale = 1f)
        {
            Addressables.LoadAssetAsync<AudioClip>(key).Completed += handle =>
            {
                AudioClip clip = handle.Result;
                audioSource.PlayOneShot(clip, volumeScale);
            };
        }
    }
}
