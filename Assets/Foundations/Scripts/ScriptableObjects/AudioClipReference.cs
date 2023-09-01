using System.Collections;
using System.Collections.Generic;
using Elysia.Animations;
using UnityEngine;

namespace Elysia
{
    [CreateAssetMenu(fileName = "AudioClipReference", menuName = "Scriptable Objects/" + "AudioClip Reference")]
    public partial class AudioClipReference : ScriptableObject
    {
        [SerializeField] private Data[] _data;

        [Header("Properties")]
        [SerializeField] private bool _loop;
        public bool Loop => _loop;

        [SerializeField] private EStopMethod _stopMethod;
        public EStopMethod StopMethod => _stopMethod;

        [SerializeField] private List<string> _stringParameters;
        public IReadOnlyList<string> StringParameters => _stringParameters;

        public void RefreshAssetKey()
        {
            if (_data == null)
            {
                return;
            }

            for (int i = 0; i < _data.Length; i++)
            {
                _data[i].RefreshKey();
            }

#if UNITY_EDITOR
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
        }

        public IData GetRandomData()
        {
            return _data[Random.Range(0, _data.Length)];
        }
    }
}
