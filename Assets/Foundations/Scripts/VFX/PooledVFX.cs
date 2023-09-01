using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledVFX : MonoBehaviour, IPooledVFX
    {
        public string Key { get; private set; }

        public bool IsPlaying => _particle.isPlaying;

        private ParticleSystem _particle;
        private Action<PooledVFX> _onParticleStopped;

        public void Initialize(string key, Action<PooledVFX> onParticleStopped)
        {
            Key = key;
            _onParticleStopped = onParticleStopped;

            _particle = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = _particle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void ReturnToPool()
        {
            if (IsPlaying)
            {
                _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public void OnParticleSystemStopped()
        {
            _onParticleStopped?.Invoke(this);
        }
    }
}
