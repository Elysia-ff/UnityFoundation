using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Elysia.Audios
{
    public class AudioManager : MonoBehaviour
    {
        private readonly struct MixerData
        {
            public readonly AudioMixerGroup mixerGroup;
            public readonly string volumeKey;

            public MixerData(AudioMixerGroup _mixerGroup, string _volumeKey)
            {
                mixerGroup = _mixerGroup;
                volumeKey = _volumeKey;
            }
        }

        public float MasterVolume
        {
            get
            {
                _audioMixer.GetFloat(_mixerGroups[EMixerType.Master].volumeKey, out float decibel);

                return DecibelToVolume(decibel);
            }
            set
            {
                float decibel = VolumeToDecibel(value);

                _audioMixer.SetFloat(_mixerGroups[EMixerType.Master].volumeKey, decibel);
            }
        }

        public float BGMVolume
        {
            get
            {
                _audioMixer.GetFloat(_mixerGroups[EMixerType.BGM].volumeKey, out float decibel);

                return DecibelToVolume(decibel);
            }
            set
            {
                float decibel = VolumeToDecibel(value);

                _audioMixer.SetFloat(_mixerGroups[EMixerType.BGM].volumeKey, decibel);
            }
        }

        public float SFXVolume
        {
            get
            {
                _audioMixer.GetFloat(_mixerGroups[EMixerType.SFX].volumeKey, out float decibel);

                return DecibelToVolume(decibel);
            }
            set
            {
                float decibel = VolumeToDecibel(value);

                _audioMixer.SetFloat(_mixerGroups[EMixerType.SFX].volumeKey, decibel);
            }
        }

        private AudioMixer _audioMixer;
        private readonly Dictionary<EMixerType, MixerData> _mixerGroups = new Dictionary<EMixerType, MixerData>();

        private readonly Dictionary<string, AudioClip> _cachedClips = new Dictionary<string, AudioClip>();

        public AudioManager Initialize()
        {
            _audioMixer = Resources.Load<AudioMixer>("Audios/AudioMixer");

            for (EMixerType m = 0; m < EMixerType.Count; m++)
            {
                AudioMixerGroup group = _audioMixer.FindMatchingGroups(m.ToString())[0];
                Debug.Assert(group != null);
                _mixerGroups.Add(m, new MixerData(group, $"{m}Volume"));
            }

            return this;
        }

        public void CreateAudioSource(string key, GameObject parent, EMixerType mixerType, bool loop, Action<AudioSource> onCompleted)
        {
            GetClipAsync(key, clip =>
            {
                AudioSource audioSource = CreateAudioSource(clip, parent, mixerType, loop);

                onCompleted?.Invoke(audioSource);
            });
        }

        public AudioSource CreateAudioSource(AudioClip clip, GameObject parent, EMixerType mixerType, bool loop)
        {
            AudioSource audioSource = parent.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = _mixerGroups[mixerType].mixerGroup;
            audioSource.playOnAwake = false;
            audioSource.loop = loop;

            return audioSource;
        }

        public void GetClipAsync(string key, Action<AudioClip> onCompleted)
        {
            if (_cachedClips.TryGetValue(key, out AudioClip clip))
            {
                onCompleted(clip);
                return;
            }

            Addressables.LoadAssetAsync<AudioClip>(key).Completed += handle =>
            {
                AudioClip clip = handle.Result;
                _cachedClips.TryAdd(key, clip);

                Addressables.Release(handle);
            };
        }

        private static float VolumeToDecibel(float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1f);
            float decibel = 20f * Mathf.Log10(volume);

            return decibel;
        }

        private static float DecibelToVolume(float decibel)
        {
            return Mathf.Pow(10f, decibel / 20f);
        }
    }
}
