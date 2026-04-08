using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.VFX
{
    public readonly struct VFXRef
    {
        private readonly VFXBase _vfx;
        private readonly uint _version;

        public bool IsValid => _vfx != null && _vfx.Version == _version;
        public VFXBase VFX => IsValid ? _vfx : null;

        public StringID Key => _vfx.Key;

        public VFXRef(VFXBase vfx)
        {
            _vfx = vfx;
            _version = _vfx.Version;
        }

        public void Stop()
        {
            if (_vfx.Version != _version)
            {
                return;
            }

            _vfx.Stop();
        }

        public void StopEmitting()
        {
            if (_vfx.Version != _version)
            {
                return;
            }

            _vfx.StopEmitting();
        }

        public void StopEmittingDetached()
        {
            if (_vfx.Version != _version)
            {
                return;
            }

            _vfx.StopEmittingDetached();
        }
    }
}
