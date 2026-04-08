using System;
using System.Globalization;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Elysia
{
    public static class DeviceHelper
    {
        public static int GetDeviceMajorVersion()
        {
#if UNITY_IOS
            try
            {
                var version = new Version(Device.systemVersion);
                return version.Major;
            }
            catch
            {
                return 0;
            }
#else
            return 0;
#endif
        }

        public static string TwoLetterLanguageName()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }

        public static bool IsSSAIDSupported()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                string id = GetSSAID();
                if (string.IsNullOrEmpty(id))
                {
                    return false;
                }

                for (int i = 0; i < id.Length; i++)
                {
                    char ch = id[i];
                    switch (ch)
                    {
                        case >= '0' and <= '9':
                        case >= 'a' and <= 'f':
                        case >= 'A' and <= 'F':
                        case '_':
                        case '-':
                        {
                            break;
                        }

                        default:
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }

        public static string GetSSAID()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string id = secure.CallStatic<string>("getString", contentResolver, "android_id");

            return id;

#else
            throw new System.NotSupportedException();
#endif
        }
    }
}
