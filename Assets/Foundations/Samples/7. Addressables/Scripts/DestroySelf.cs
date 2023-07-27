using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float _time = 5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_time);

        Addressables.ReleaseInstance(gameObject);
    }
}
