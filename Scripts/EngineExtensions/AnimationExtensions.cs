using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class AnimationExtensions
    {
        public static void AddEvent(this AnimationClip clip, float time, string functionName)
        {
            AnimationEvent newEvent = new AnimationEvent
            {
                time = time,
                functionName = functionName,
            };

            clip.AddEvent(newEvent);
        }

        public static void AddEvent(this AnimationClip clip, float time, string functionName, string stringParameter)
        {
            AnimationEvent newEvent = new AnimationEvent
            {
                time = time,
                functionName = functionName,
                stringParameter = stringParameter
            };

            clip.AddEvent(newEvent);
        }
    }
}
