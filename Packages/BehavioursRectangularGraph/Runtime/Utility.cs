using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public static IEnumerable<T> GetWeightedShuffle<T>(this IEnumerable<T> values, System.Func<T, float> getWeight)
        {
            var valuesCopy = new HashSet<T>(values);

            var result = new List<T>();
            while (valuesCopy.Count > 0)
            {
                var totalWeight = values.Sum(v => getWeight(v));
                var itemWeightIndex = Random.value * totalWeight;
                var currentWeightIndex = 0f;
                foreach (var item in from weightedItem in valuesCopy select weightedItem)
                {
                    currentWeightIndex += getWeight(item);

                    // If we've hit or passed the weight we are after for this item then it's the one we want....
                    if (currentWeightIndex > itemWeightIndex)
                    {
                        yield return item;
                        valuesCopy.Remove(item);
                        break;
                    }
                }
            }
        }

        public struct CycleCheckInfo
        {
            public RectangularDirection direction;
            public int exitIndex;
            public RectangularDirection otherDirection;
            public int otherExitIndex;
        }
    }
}