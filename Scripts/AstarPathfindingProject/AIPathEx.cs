// This is dependent on AstarPathfindingProject package.
// Comment out this file if the package is not included.
#if ASTAR_INCLUDED

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AIPathEx : AIPath
    {
        public enum EPathType
        {
            AB,
            Flee,
            Random,
            FloodPathTracer
        }

        public EPathType pathType = EPathType.AB;
        public int searchLength = 20 * 1000;
        public float aimStrength = 0.5f;
        public int spread = 5000;

        private FloodPath _floodPath;

        public override void SearchPath()
        {
            if ((pathType == EPathType.AB || pathType == EPathType.Flee) && float.IsPositiveInfinity(destination.x))
            {
                return;
            }

            onSearchPath?.Invoke();

            CalculatePathRequestEndpoints(out Vector3 start, out Vector3 end);

            // Request a path to be calculated from our current position to the destination
            Path p = CreatePath(start, end);
            SetPath(p, false);
        }

        public void SearchFloodPath(Vector3 end)
        {
            _floodPath = FloodPath.Construct(end);
            AstarPath.StartPath(_floodPath);
        }

        /// <summary>
        /// Use below code snippet to create <paramref name="floodPath"/>
        /// <code>
        /// var p = FloodPath.Construct(point);
        /// AstarPath.StartPath(p);
        /// </code>
        /// </summary>
        /// <param name="floodPath">pre-calculated <see cref="FloodPath"/></param>
        public void SetFloodPath(FloodPath floodPath)
        {
            _floodPath = floodPath;
        }

        public void NullifyFloodPath()
        {
            _floodPath = null;
        }

        private Path CreatePath(Vector3 start, Vector3 end)
        {
            switch (pathType)
            {
                case EPathType.AB:
                {
                    return ABPath.Construct(start, end);
                }

                case EPathType.Flee:
                {
                    FleePath fleePath = FleePath.Construct(start, end, searchLength);
                    fleePath.aimStrength = aimStrength;
                    fleePath.spread = spread;

                    return fleePath;
                }

                case EPathType.Random:
                {
                    RandomPath randomPath = RandomPath.Construct(start, searchLength);
                    randomPath.spread = spread;
                    randomPath.aimStrength = aimStrength;
                    randomPath.aim = end;

                    return randomPath;
                }

                case EPathType.FloodPathTracer when _floodPath != null && _floodPath.PipelineState >= PathState.Returning:
                {
                    return FloodPathTracer.Construct(start, _floodPath);
                }
            }

            return null;
        }
    }
}
#endif
