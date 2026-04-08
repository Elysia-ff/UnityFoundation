using System.Collections.Generic;
using System.Threading.Tasks;
using Elysia.Pools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;

namespace Elysia.Audios
{
    public class AudioManager : MonoBehaviour
    {
        public static readonly AudioClip NULL_CLIP = null;

        private readonly struct MixerData
        {
            public readonly AudioMixerGroup mixerGroup;
            public readonly string volumeKey;

            public MixerData(AudioMixerGroup mixerGroup, string volumeKey)
            {
                this.mixerGroup = mixerGroup;
                this.volumeKey = volumeKey;
            }
        }

        public bool IsInitialized { get; private set; }

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

        private AudioSource _defaultSource2D;
        private AudioSource _defaultSource3D;

        private ObjectPool<OneShotClip> _oneShotClip2DPool;
        private ObjectPool<OneShotClip> _oneShotClip3DPool;

        public async Task InitializeAsync()
        {
            Debug.Assert(!IsInitialized);

            try
            {
                _audioMixer = await Addressables.LoadAssetAsync<AudioMixer>("AudioMixer").Task;

                Task<AudioSource> task2D = LoadDefaultAudioSource("DefaultAudioSource2D");
                Task<AudioSource> task3D = LoadDefaultAudioSource("DefaultAudioSource3D");
                await Task.WhenAll(task2D, task3D);

                _defaultSource2D = task2D.Result;
                if (_defaultSource2D != null)
                {
                    _oneShotClip2DPool = new ObjectPool<OneShotClip>(transform, () =>
                    {
                        OneShotClip oneShotClip = new GameObject(nameof(OneShotClip)).AddComponent<OneShotClip>();
                        CreateAudioSource2D(NULL_CLIP, oneShotClip.gameObject, EMixerType.SFX, false, 1.0f);
                        oneShotClip.Initialize(ReturnOneShotClip2D);

                        return oneShotClip;
                    }, false, false, 0, 16);
                }

                _defaultSource3D = task3D.Result;
                if (_defaultSource3D != null)
                {
                    _oneShotClip3DPool = new ObjectPool<OneShotClip>(transform, () =>
                    {
                        OneShotClip oneShotClip = new GameObject(nameof(OneShotClip)).AddComponent<OneShotClip>();
                        CreateAudioSource3D(NULL_CLIP, oneShotClip.gameObject, EMixerType.SFX, false, 1.0f);
                        oneShotClip.Initialize(ReturnOneShotClip3D);

                        return oneShotClip;
                    }, false, false, 0, 16);
                }

                for (EMixerType m = 0; m < EMixerType.Count; m++)
                {
                    AudioMixerGroup group = _audioMixer.FindMatchingGroups(m.ToString())[0];
                    Debug.Assert(group != null);
                    string volumeKey = $"{m}Volume";
                    _mixerGroups.Add(m, new MixerData(group, volumeKey));

                    Debug.Assert(_audioMixer.GetFloat(volumeKey, out float _));
                }
            }
            catch (InvalidKeyException)
            {
                // swallow exception
            }

            IsInitialized = true;
        }

        private async Task<AudioSource> LoadDefaultAudioSource(string key)
        {
            try
            {
                GameObject obj = await Addressables.LoadAssetAsync<GameObject>(key).Task;

                return obj.GetComponent<AudioSource>();
            }
            catch (InvalidKeyException)
            {
                // swallow exception
            }

            return null;
        }

        public AudioSource CreateAudioSource2D(StringID key, GameObject parent, EMixerType mixerType, bool loop, float volume, bool ignoreListenerPause = false)
        {
            AudioClip clip = App.ResourceHolder.GetAudioClip(key);

            return CreateAudioSource2D(clip, parent, mixerType, loop, volume, ignoreListenerPause);
        }

        public AudioSource CreateAudioSource2D(AudioClip clip, GameObject parent, EMixerType mixerType, bool loop, float volume, bool ignoreListenerPause = false)
        {
            AudioSource audioSource = CreateAudioSourceFrom(_defaultSource2D, parent, mixerType);
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.ignoreListenerPause = ignoreListenerPause;

            return audioSource;
        }

        public AudioSource CreateAudioSource3D(StringID key, GameObject parent, EMixerType mixerType, bool loop, float volume, bool ignoreListenerPause = false)
        {
            AudioClip clip = App.ResourceHolder.GetAudioClip(key);

            return CreateAudioSource3D(clip, parent, mixerType, loop, volume, ignoreListenerPause);
        }

        public AudioSource CreateAudioSource3D(AudioClip clip, GameObject parent, EMixerType mixerType, bool loop, float volume, bool ignoreListenerPause = false)
        {
            Debug.Assert(_defaultSource3D != null);

            AudioSource audioSource = CreateAudioSourceFrom(_defaultSource3D, parent, mixerType);
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.ignoreListenerPause = ignoreListenerPause;

            return audioSource;
        }

        private AudioSource CreateAudioSourceFrom(AudioSource baseProperties, GameObject parent, EMixerType mixerType)
        {
            AudioSource audioSource = parent.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _mixerGroups[mixerType].mixerGroup;
            audioSource.playOnAwake = false;

            if (baseProperties != null)
            {
                audioSource.mute = baseProperties.mute;
                audioSource.bypassEffects = baseProperties.bypassEffects;
                audioSource.bypassListenerEffects = baseProperties.bypassListenerEffects;
                audioSource.bypassReverbZones = baseProperties.bypassReverbZones;
                audioSource.loop = baseProperties.loop;
                audioSource.priority = baseProperties.priority;
                audioSource.volume = baseProperties.volume;
                audioSource.pitch = baseProperties.pitch;
                audioSource.panStereo = baseProperties.panStereo;
                audioSource.spatialBlend = baseProperties.spatialBlend;
                audioSource.reverbZoneMix = baseProperties.reverbZoneMix;
                audioSource.dopplerLevel = baseProperties.dopplerLevel;
                audioSource.spread = baseProperties.spread;
                audioSource.rolloffMode = baseProperties.rolloffMode;
                audioSource.minDistance = baseProperties.minDistance;
                audioSource.maxDistance = baseProperties.maxDistance;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, baseProperties.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
                audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, baseProperties.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
                audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, baseProperties.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
                audioSource.SetCustomCurve(AudioSourceCurveType.Spread, baseProperties.GetCustomCurve(AudioSourceCurveType.Spread));
            }

            return audioSource;
        }

        public void PlayClipAtPoint2D(StringID key, Vector3 position, float volume = 1f, bool ignoreListenerPause = false)
        {
            AudioClip clip = App.ResourceHolder.GetAudioClip(key);
            PlayClipAtPoint2D(clip, position, volume, ignoreListenerPause);
        }

        public void PlayClipAtPoint2D(AudioClip clip, Vector3 position, float volume = 1f, bool ignoreListenerPause = false)
        {
            OneShotClip oneShotClip = _oneShotClip2DPool.Get();
            oneShotClip.OnStart(clip, position, volume, ignoreListenerPause);
        }

        private void ReturnOneShotClip2D(OneShotClip oneShotClip)
        {
            _oneShotClip2DPool.Release(ref oneShotClip);
        }

        public void PlayClipAtPoint3D(StringID key, Vector3 position, float volume = 1f, bool ignoreListenerPause = false)
        {
            AudioClip clip = App.ResourceHolder.GetAudioClip(key);
            PlayClipAtPoint3D(clip, position, volume, ignoreListenerPause);
        }

        public void PlayClipAtPoint3D(AudioClip clip, Vector3 position, float volume = 1f, bool ignoreListenerPause = false)
        {
            OneShotClip oneShotClip = _oneShotClip3DPool.Get();
            oneShotClip.OnStart(clip, position, volume, ignoreListenerPause);
        }

        private void ReturnOneShotClip3D(OneShotClip oneShotClip)
        {
            _oneShotClip3DPool.Release(ref oneShotClip);
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