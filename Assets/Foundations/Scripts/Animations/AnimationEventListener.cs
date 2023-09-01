using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elysia.Audios;
using UnityEngine;

namespace Elysia.Animations
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventListener : MonoBehaviour
    {
        private string _currentAnimation;

        private AudioClipReference _loopingAudioRef;
        private AudioSource _loopingAudioSource;
        private Coroutine _loopingAudioSourceRoutine;
        private AudioSource _oneShotAudioSource;

        private static readonly HashSet<AnimationClip> PREPROCESSED_CLIPS = new HashSet<AnimationClip>();

        private void Awake()
        {
            _loopingAudioSource = Game.Audio.CreateAudioSource(null, gameObject, EMixerType.SFX, false, 1f);
            _oneShotAudioSource = Game.Audio.CreateAudioSource(null, gameObject, EMixerType.SFX, false, 1f);

            Animator animator = GetComponent<Animator>();
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (PREPROCESSED_CLIPS.Contains(clip))
                {
                    continue;
                }

                clip.AddEvent(0, nameof(OnAnimationClipStarted), clip.name);
                //clip.AddEvent(clip.length, nameof(OnAnimationClipEnded), clip.name);

                PREPROCESSED_CLIPS.Add(clip);
            }
        }

        private IEnumerator LoopingAudioSourceRoutine()
        {
            while (_loopingAudioRef != null)
            {
                if (!_loopingAudioSource.isPlaying)
                {
                    AudioClipReference.IData audioData = _loopingAudioRef.GetRandomData();
                    Game.Audio.GetClipAsync(audioData.Key, clip =>
                    {
                        _loopingAudioSource.clip = clip;
                        _loopingAudioSource.volume = audioData.VolumeOverride;
                        _loopingAudioSource.Play();
                    });
                }

                yield return null;
            }

            _loopingAudioSourceRoutine = null;
        }

        public void PlaySFX(AudioClipReference assetReference)
        {
            Debug.Assert(assetReference != null);

            AudioClipReference.IData audioData = assetReference.GetRandomData();
            Game.Audio.GetClipAsync(audioData.Key, clip =>
            {
                if (assetReference.Loop)
                {
                    if (_loopingAudioRef == null || _loopingAudioRef != assetReference)
                    {
                        _loopingAudioRef = assetReference;
                        _loopingAudioSource.clip = clip;
                        _loopingAudioSource.volume = audioData.VolumeOverride;
                        _loopingAudioSource.Play();

                        if (_loopingAudioSourceRoutine != null)
                        {
                            StopCoroutine(_loopingAudioSourceRoutine);
                        }
                        _loopingAudioSourceRoutine = StartCoroutine(LoopingAudioSourceRoutine());
                    }
                }
                else
                {
                    _oneShotAudioSource.PlayOneShot(clip, audioData.VolumeOverride);
                }
            });
        }

        private void OnAnimationClipStarted(string clipName)
        {
            if (_currentAnimation == clipName)
            {
                return;
            }

            _currentAnimation = clipName;

            if (_loopingAudioRef != null)
            {
                switch (_loopingAudioRef.StopMethod)
                {
                    case EStopMethod.OnOtherAnimationStarted:
                        StopLoopingAudio();
                        break;

                    case EStopMethod.OnAnimationContainsGivenStringStarted:
                        if (_loopingAudioRef.StringParameters.Any(clipName.Contains))
                        {
                            StopLoopingAudio();
                        }
                        break;

                    case EStopMethod.OnAnimationDoesNotContainsGivenStringStarted:
                        if (!_loopingAudioRef.StringParameters.Any(clipName.Contains))
                        {
                            StopLoopingAudio();
                        }
                        break;
                }
            }
        }

        private void StopLoopingAudio()
        {
            _loopingAudioRef = null;
            _loopingAudioSource.clip = null;

            if (_loopingAudioSourceRoutine != null)
            {
                StopCoroutine(_loopingAudioSourceRoutine);
                _loopingAudioSourceRoutine = null;
            }
        }

        // private void OnAnimationClipEnded(string clipName)
        // {
        // }
    }
}
