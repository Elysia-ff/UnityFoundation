using Elysia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Starter : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private GameObject _cube;

    public Starter Initialize()
    {
        _director.played += (director) => Debug.Log("played");
        _director.stopped += (director) => Debug.Log("stopped");

        _director.MakeBuilder()
            .AddMethod("Tween", (trackAsset) =>
            {
                _director.SetGenericBinding(trackAsset, _cube.transform);

                TransformTweenClip clip = trackAsset.GetAssetAt<TransformTweenClip>(0);
                _director.SetReferenceValue(clip.startLocation.exposedName, _startPosition);
                _director.SetReferenceValue(clip.endLocation.exposedName, _endPosition);
            })
            .Build();

        return this;
    }

    public void Run()
    {
        _director.Play();
    }
}
