using System.Collections.Generic;
using UnityEngine;

namespace BehavioursRectangularGraph
{
    public abstract class RectangularNodeBehavior : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 1)] private float m_SpawnChance = 1;

        public float SpawnChance => m_SpawnChance * 100;

        #region Exits

        public abstract IReadOnlyList<Transform> TopExits { get; }
        public abstract IReadOnlyList<Transform> RightExits { get; }
        public abstract IReadOnlyList<Transform> LeftExits { get; }
        public abstract IReadOnlyList<Transform> BottomExits { get; }

        #endregion

        public abstract bool IsCompatible(
            Vector3 behaviourWorldPosition,
            RectangularNodeBehavior otherBehavior,
            Vector3 otherBehaviorWorldPosition
            );


        public bool TryGetCycles(
            Vector3 behaviourWorldPosition,
            RectangularNodeBehavior otherBehavior,
            Vector3 otherBehaviorWorldPosition,
            out List<Utility.CycleCheckInfo> cycleChecks)
        {
            cycleChecks = new List<Utility.CycleCheckInfo>();
            foreach (var direction in Utility.GetEachDirection())
            {
                var inverseDirection = Utility.GetInversedDirection(direction);
                var exits = GetExitsByDirection(direction);
                var otherExits = otherBehavior.GetExitsByDirection(inverseDirection);
                for (int i = 0; i < exits.Count; i++)
                {
                    var exitWorldPosition = GetExitWorldPosition(direction, i, behaviourWorldPosition);
                    for (int j = 0; j < otherExits.Count; j++)
                    {
                        var otherExitWorldPosition = otherBehavior
                            .GetExitWorldPosition(inverseDirection, j, otherBehaviorWorldPosition);
                        if (exitWorldPosition == otherExitWorldPosition)
                        {
                            cycleChecks.Add(new Utility.CycleCheckInfo
                            { 
                                    direction = direction, 
                                    exitIndex = i,
                                    otherDirection = inverseDirection,
                                    otherExitIndex = j
                                }
                            );
                        }
                    }
                }
            }
            return cycleChecks.Count > 0;
        }

        public Vector3 GetExitWorldPosition(
            RectangularDirection exitDirection,
            int exitIndex,
            Vector3 nodeWorldPosition)
        {
            var currentExitPosition = GetExitsByDirection(exitDirection)[exitIndex].position;
            var delta = transform.InverseTransformPoint(currentExitPosition);
            return nodeWorldPosition + delta;
        }

        public IReadOnlyList<Transform> GetExitsByDirection(RectangularDirection direction)
        {
            return direction switch
            {
                RectangularDirection.Left => LeftExits,
                RectangularDirection.Up => TopExits,
                RectangularDirection.Right => RightExits,
                RectangularDirection.Down => BottomExits,
                _ => throw new System.NotImplementedException()
            };
        }

        public Vector3 GetWorldPositionRelatively(
            RectangularDirection exitDirection,
            int exitIndex,
            Vector3 exitWorldPosition)
        {
            var currentExitPosition = GetExitsByDirection(exitDirection)[exitIndex].position;
            var delta = -transform.InverseTransformPoint(currentExitPosition);
            return exitWorldPosition + delta;
        }
    }
}