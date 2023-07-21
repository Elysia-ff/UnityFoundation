using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public abstract class SceneBase : MonoBehaviour
    {
        public int Handle { get; private set; }

        public virtual void Initialize(int handle)
        {
            Handle = handle;
        }
    }
}
