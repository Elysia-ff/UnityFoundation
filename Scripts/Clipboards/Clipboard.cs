using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class Clipboard
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        private static AndroidJavaObject _clipboard;
#elif UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void CopyToClipboard_Elysia(string str);
#endif

        public static void Copy(string text)
        {
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = text;
#elif UNITY_ANDROID
            if (_clipboard == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass staticContext = new AndroidJavaClass("android.content.Context");
                AndroidJavaObject service = staticContext.GetStatic<AndroidJavaObject>("CLIPBOARD_SERVICE");
                _clipboard = activity.Call<AndroidJavaObject>("getSystemService", service);
            }

            _clipboard.Call("setText", text);
#elif UNITY_IOS
            CopyToClipboard_Elysia(text);
#endif
        }
    }
}
