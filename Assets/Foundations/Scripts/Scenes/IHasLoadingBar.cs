using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public interface IHasLoadingBar
    {
        void OnLoadingInProgress(float progress);
    }
}
