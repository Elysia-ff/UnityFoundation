using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

public static class Debug
{
    [Conditional("UNITY_ASSERTIONS")]
    [AssertionMethod]
    public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    [Conditional("UNITY_ASSERTIONS")]
    [AssertionMethod]
    public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, object message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    [AssertionMethod]
    public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, object message, Object context)
    {
        UnityEngine.Debug.Assert(condition, message, context);
    }

#if !DISABLE_LOG
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
#endif
    [Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

#if !DISABLE_LOG
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
#endif
    [Conditional("ENABLE_LOG")]
    public static void Log(object message, Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

#if !DISABLE_LOG
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
#endif
    [Conditional("ENABLE_LOG")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

#if !DISABLE_LOG
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
#endif
    [Conditional("ENABLE_LOG")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

#if !DISABLE_LOG
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
#endif
    [Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }
}
