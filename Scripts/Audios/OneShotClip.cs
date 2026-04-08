using UnityEngine;

namespace Elysia.Audios
{
    public class OneShotClip : MonoBehaviour
    {
        private AudioSource _source;

        private System.Action<OneShotClip> _onStopped;

        public void Initialize(System.Action<OneShotClip> onStopped)
        {
            Debug.Assert(onStopped != null);

            _source = GetComponent<AudioSource>();

            _onStopped = onStopped;
        }

        public void OnStart(AudioClip clip, Vector3 position, float volume, bool ignoreListenerPause)
        {
            Debug.Assert(!_source.isPlaying);

            transform.position = position;

            _source.clip = clip;
            _source.volume = volume;
            _source.ignoreListenerPause = ignoreListenerPause;
            _source.Play();
        }

        public void Stop()
        {
            _onStopped(this);
        }

        private void Update()
        {
            if (!_source.isPlaying && (_source.ignoreListenerPause || !AudioListener.pause))
            {
                _onStopped(this);
            }
        }
    }
}
