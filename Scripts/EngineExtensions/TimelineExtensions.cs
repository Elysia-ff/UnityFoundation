using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Elysia
{
    public static class TimelineExtensions
    {
        public class Builder
        {
            public delegate void BuilderDelegate(TrackAsset trackAsset);

            private readonly TimelineAsset _timelineAsset;
            private readonly Dictionary<string, BuilderDelegate> _methods = new Dictionary<string, BuilderDelegate>();

            public Builder(PlayableDirector director)
            {
                _timelineAsset = (TimelineAsset)director.playableAsset;
            }

            public Builder AddMethod(string key, BuilderDelegate method)
            {
                _methods.Add(key, method);

                return this;
            }

            public void Build()
            {
                foreach (var kv in _timelineAsset.GetRootTracks())
                {
                    if (_methods.TryGetValue(kv.name, out BuilderDelegate method))
                    {
                        TrackAsset trackAsset = kv.GetChildTracks().ElementAt(0);
                        method(trackAsset);
                    }
                }
            }
        }

        public static Builder MakeBuilder(this PlayableDirector director)
        {
            return new Builder(director);
        }

        public static T GetAssetAt<T>(this TrackAsset trackAsset, int index)
            where T : UnityEngine.Object
        {
            return (T)trackAsset.GetClips().ElementAt(index).asset;
        }
    }
}
