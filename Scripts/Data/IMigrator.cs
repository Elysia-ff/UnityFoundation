using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public interface IMigrator<out T>
        where T : SaveData
    {
        T Migrate();
    }
}
