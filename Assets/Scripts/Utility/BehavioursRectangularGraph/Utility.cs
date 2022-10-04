using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BehavioursRectangularGraph
{
    public static class Utility
    {
        public static RectangularDirection GetInversedDirection(RectangularDirection direction) =>
            (RectangularDirection)(-(int)direction);

        public static IEnumerable<RectangularDirection> GetEachDirection()
        {
            return System.Enum.GetValues(typeof(RectangularDirection)) as IEnumerable<RectangularDirection>;
        }

        public static Vector3 GetCorrespondingVector(RectangularDirection direction)
        {
            return direction switch
            {
                RectangularDirection.Left => Vector3.left,
                RectangularDirection.Up => Vector3.up,
                RectangularDirection.Right => Vector3.right,
                RectangularDirection.Down => Vector3.down,
                _ => throw new System.NotImplementedException(),
            };
        }

        public struct CycleCheckInfo
        {
            public RectangularDirection direction;
            public int exitIndex;
            public RectangularDirection otherDirection;
            public int otherExitIndex;
        }

        public static List<T> GetWeightedShuffle<T>(List<T> values) where T : IWeight
        {
            var valuesCopy = new HashSet<T>();
            foreach (var value in values)
            {
                valuesCopy.Add(value);
            }

            var result = new List<T>();
            while(valuesCopy.Count > 0)
            {
                var totalWeight = values.Sum(v => v.Weight);
                var itemWeightIndex = Random.value * totalWeight;
                var currentWeightIndex = 0f;
                foreach (var item in from weightedItem in valuesCopy select weightedItem)
                {
                    currentWeightIndex += item.Weight;

                    // If we've hit or passed the weight we are after for this item then it's the one we want....
                    if (currentWeightIndex > itemWeightIndex)
                    {
                        result.Add(item);
                        valuesCopy.Remove(item);
                        break;
                    }

                }
            }

            return result;
        }

        public interface IWeight
        {
            public float Weight { get; }
        }
    }
}