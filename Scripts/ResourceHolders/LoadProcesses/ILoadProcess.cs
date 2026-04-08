using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.ResourceManagement
{
    public interface ILoadProcess
    {
        int Flag { get; }

        EPhaseType Phase { get; }
        float PercentComplete { get; }

        void Unload();

        void WaitForLocations();

        IEnumerator WaitForLocationsAsync();

        void LoadAssets();

        void WaitForAssets();

        bool WaitForAssetsAsync();
    }
}
