using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.VFX
{
    public abstract class VFXBase : MonoBehaviour
    {
        public StringID Key { get; private set; }

        public abstract bool IsPlaying { get; }
        public uint Version { get; private set; }

        private bool _isAlive;

        private static uint VERSION = 1;

        public virtual void Initialize(StringID key)
        {
            Key = key;
        }

        public virtual void OnStart()
        {
            _isAlive = true;

            unchecked
            {
                Version = VERSION++;
            }
        }

        protected virtual void OnDisable()
        {
            if (_isAlive)
            {
                // Re-parenting in OnDisable is not allowed
                // this will call Stop() in next frame
                App.Scene.VFX.EnqueueDisabledVFX(this);
            }
        }

        public void Stop()
        {
            if (!_isAlive)
            {
                return;
            }

            _isAlive = false;
            App.Scene.VFX.OnVFXStopped(this);
        }

        public void StopEmitting()
        {
            StopEmitting_Impl();
        }

        public void StopEmittingDetached()
        {
            transform.SetParent(null);
            StopEmitting_Impl();
        }

        protected virtual void StopEmitting_Impl()
        {
            Stop();
        }
    }
}
