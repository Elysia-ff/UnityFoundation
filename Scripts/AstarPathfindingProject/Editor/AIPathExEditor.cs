// This is dependent on AstarPathfindingProject package.
// Comment out this file if the package is not included.
#if ASTAR_INCLUDED

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{
    [CustomEditor(typeof(AIPathEx), true)]
    [CanEditMultipleObjects]
    public class AIPathExEditor : BaseAIEditor
    {
        protected override void Inspector()
        {
            Section("Path Type");
            Popup("pathType", System.Enum.GetNames(typeof(AIPathEx.EPathType)).Select(n => new GUIContent(n)).ToArray());

            SerializedProperty pathTypeProperty = FindProperty("pathType");
            AIPathEx.EPathType pathType = (AIPathEx.EPathType)pathTypeProperty.enumValueIndex;
            switch (pathType)
            {
                case AIPathEx.EPathType.Flee:
                case AIPathEx.EPathType.Random:
                    IntField("searchLength", tooltip: "World Unit * 1000", min: 0, max: 100000);
                    FloatField("aimStrength", min: 0f, max: 1f);
                    IntField("spread", min: 0, max: 40000);
                    break;
            }

            base.Inspector();
        }

        protected void IntField (string propertyPath, string label = null, string tooltip = null, int min = int.MinValue, int max = int.MaxValue) {
            PropertyField(propertyPath, label, tooltip);
            ClampInt(propertyPath, min, max);
        }
    }
}
#endif
