#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Elysia.VFX;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    public class VFXTester : MonoBehaviour
    {
        [SerializeField] private ComponentReferenceT<VFXBase> _vfxLoop;
        [SerializeField] private ComponentReferenceT<VFXBase> _vfxOnce;

        [Space(10f)]
        [SerializeField] private Transform _parent;

        private readonly Queue<VFXRef> _loops = new Queue<VFXRef>(8);
        private readonly Queue<VFXRef> _onces = new Queue<VFXRef>(8);

        public void CreateLoop()
        {
            VFXRef vfx = App.Scene.VFX.CreateAt(_vfxLoop.Key, _parent, _parent.position);
            _loops.Enqueue(vfx);
        }

        public void StopLoop(bool stopEmitting, bool detach)
        {
            if (_loops.Count > 0)
            {
                VFXRef vfx = _loops.Dequeue();
                StopVFX(vfx, stopEmitting, detach);
            }
        }

        public void CreateOnce()
        {
            VFXRef vfx = App.Scene.VFX.CreateAt(_vfxOnce.Key, _parent, _parent.position);
            _onces.Enqueue(vfx);
        }

        public void StopOnce(bool stopEmitting, bool detach)
        {
            if (_onces.Count > 0)
            {
                VFXRef vfx = _onces.Dequeue();
                StopVFX(vfx, stopEmitting, detach);
            }
        }

        public void EnableParent()
        {
            _parent.gameObject.SetActive(true);
        }

        public void DisableParent()
        {
            _parent.gameObject.SetActive(false);
        }

        public int LoopCount()
        {
            return _loops.Count;
        }

        public int OnceCount()
        {
            return _onces.Count;
        }

        private void StopVFX(VFXRef vfx, bool stopEmitting, bool detach)
        {
            if (stopEmitting)
            {
                vfx.StopEmitting();

                if (detach)
                {
                    vfx.StopEmittingDetached();
                }
                else
                {
                    vfx.StopEmitting();
                }
            }
            else
            {
                vfx.Stop();
            }
        }
    }

    [CustomEditor(typeof(VFXTester))]
    public class VFXTesterEditor : Editor
    {
        private static bool _stopEmitting;
        private static bool _detach;
        private static bool _disableParent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using var _ = new EditorGUI.DisabledScope(!Application.isPlaying);

            VFXTester tester = (VFXTester)target;

            GUILayout.Space(20f);

            _stopEmitting = EditorGUILayout.Toggle("Stop Emitting", _stopEmitting);
            _detach = EditorGUILayout.Toggle("Detach", _detach);
            _disableParent = EditorGUILayout.Toggle("Disable Parent", _disableParent);

            GUILayout.Space(10f);

            GUILayout.Label("Loop Count: " + tester.LoopCount());
            GUILayout.Label("Once Count: " + tester.OnceCount());

            GUILayout.Space(10f);

            if (GUILayout.Button("Create Loop"))
            {
                tester.CreateLoop();
            }

            if (GUILayout.Button("Stop Loop"))
            {
                tester.StopLoop(_stopEmitting, _detach);

                if (_disableParent)
                {
                    tester.DisableParent();
                }
            }

            if (GUILayout.Button("Create Once"))
            {
                tester.CreateOnce();
            }

            if (GUILayout.Button("Stop Once"))
            {
                tester.StopOnce(_stopEmitting, _detach);

                if (_disableParent)
                {
                    tester.DisableParent();
                }
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Enable Parent"))
            {
                tester.EnableParent();
            }

            if (GUILayout.Button("Disable Parent"))
            {
                tester.DisableParent();
            }
        }
    }
}
#endif
