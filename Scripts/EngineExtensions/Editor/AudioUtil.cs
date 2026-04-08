using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Audio/Bindings/AudioUtil.bindings.cs
    public static class AudioUtil
    {
        private static Texture2D _playTexture;
        public static Texture2D PlayTexture
        {
            get
            {
                if (_playTexture == null)
                {
                    _playTexture = EditorGUIUtility.FindTexture("d_PlayButton");
                }

                return _playTexture;
            }
        }

        private static Texture2D _stopTexture;
        public static Texture2D StopTexture
        {
            get
            {
                if (_stopTexture == null)
                {
                    _stopTexture = EditorGUIUtility.FindTexture("d_PreMatQuad");
                }

                return _stopTexture;
            }
        }

        public static void PlayPreviewClip(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            Assembly assembly = typeof(AudioImporter).Assembly;

            Type type = assembly.GetType("UnityEditor.AudioUtil");
            Reflection.CallMethod(type, "PlayPreviewClip", new object[] { clip, 0, false });
        }

        public static void StopAllPreviewClips()
        {
            Assembly assembly = typeof(AudioImporter).Assembly;

            Type type = assembly.GetType("UnityEditor.AudioUtil");
            Reflection.CallMethod(type, "StopAllPreviewClips");
        }

        public static bool IsPreviewClipPlaying()
        {
            Assembly assembly = typeof(AudioImporter).Assembly;

            Type type = assembly.GetType("UnityEditor.AudioUtil");
            return Reflection.CallMethod<bool>(type, "IsPreviewClipPlaying");
        }
    }
}
