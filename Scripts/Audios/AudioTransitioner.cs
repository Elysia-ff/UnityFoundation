using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Audios
{
    public class AudioTransitioner : MonoBehaviour
    {
        private struct Data
        {
            public readonly int id;
            public float volume;

            public Data(int id)
            {
                this.id = id;
                this.volume = 1f;
            }

            public Data(int id, float volume)
            {
                this.id = id;
                this.volume = volume;
            }
        }

        private readonly struct Message
        {
            public readonly StringID clipName;
            public readonly bool loop;
            public readonly float volume;
            public readonly float transitionTime;

            public Message(StringID clipName, bool loop, float volume, float transitionTime)
            {
                this.clipName = clipName;
                this.loop = loop;
                this.volume = volume;
                this.transitionTime = transitionTime;
            }
        }

        private readonly AudioSource[] _buffers = new AudioSource[2];
        private Data _currentAudioSource = new Data(0);
        private Data _bufferAudioSource = new Data(1);
        private Coroutine _transitionRoutine;
        private readonly Queue<Message> _queue = new Queue<Message>();

        private AnimationCurve _blendInCurve;
        private AnimationCurve _blendOutCurve;

        public void Initialize2D(EMixerType mixerType, AnimationCurve blendInCurve, AnimationCurve blendOutCurve, bool ignoreListenerPause)
        {
            _buffers[0] = App.Audio.CreateAudioSource2D(AudioManager.NULL_CLIP, gameObject, mixerType, false, 1f, ignoreListenerPause);
            _buffers[1] = App.Audio.CreateAudioSource2D(AudioManager.NULL_CLIP, gameObject, mixerType, false, 1f, ignoreListenerPause);

            _blendInCurve = blendInCurve;
            _blendOutCurve = blendOutCurve;
        }

        private void Update()
        {
            if (_queue.Count == 0)
            {
                return;
            }

            AudioSource audioSource = _buffers[_currentAudioSource.id];
            Message message = _queue.Peek();
            if (audioSource.clip != null && audioSource.clip.length - audioSource.time > message.transitionTime)
            {
                return;
            }

            Play(message.clipName, message.loop, message.volume, message.transitionTime);
            _queue.Dequeue();
        }

        public void Play(StringID clipName, bool loop, float volume, float transitionTime)
        {
            AudioClip clip = App.ResourceHolder.GetAudioClip(clipName);
            Play(clip, loop, volume, transitionTime);
        }

        public void Play(AudioClip clip, bool loop, float volume, float transitionTime)
        {
            _bufferAudioSource.volume = volume;

            AudioSource audioSource = _buffers[_bufferAudioSource.id];
            audioSource.clip = clip;
            audioSource.volume = 0f;
            audioSource.loop = loop;

            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
            }
            _transitionRoutine = StartCoroutine(TransitionRoutine(transitionTime));
        }

        public void Stop(float transitionTime)
        {
            Play(null, false, 0f, transitionTime);
        }

        public void Enqueue(StringID clipName, bool loop, float volume, float transitionTime)
        {
            _queue.Enqueue(new Message(clipName, loop, volume, transitionTime));
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        private IEnumerator TransitionRoutine(float time)
        {
            Data from = _currentAudioSource;
            Data to = _bufferAudioSource;
            (_currentAudioSource, _bufferAudioSource) = (_bufferAudioSource, _currentAudioSource);

            AudioSource sourceFrom = _buffers[from.id];
            AudioSource sourceTo = _buffers[to.id];
            sourceTo.volume = 0f;
            sourceTo.Play();

            if (time > 0f)
            {
                float speed = 1f / time;
                for (float t = 0; t < 1f; t += Time.deltaTime * speed)
                {
                    float outValue = _blendOutCurve.Evaluate(t);
                    sourceFrom.volume = from.volume * outValue;

                    float inValue = _blendInCurve.Evaluate(t);
                    sourceTo.volume = to.volume * inValue;

                    yield return null;
                }
            }

            sourceFrom.volume = _blendOutCurve.Evaluate(1f) * from.volume;
            sourceFrom.Stop();
            sourceTo.volume = _blendInCurve.Evaluate(1f) * to.volume;

            _transitionRoutine = null;
        }
    }
}
