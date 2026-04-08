using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXParticle : VFXBase
    {
        public override bool IsPlaying => _particle.isPlaying;

        private ParticleSystem _particle;

        public override void Initialize(StringID key)
        {
            base.Initialize(key);

            _particle = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = _particle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        protected override void StopEmitting_Impl()
        {
            // calls OnParticleSystemStopped()
            _particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void OnParticleSystemStopped()
        {
            Stop();
        }
    }
}
