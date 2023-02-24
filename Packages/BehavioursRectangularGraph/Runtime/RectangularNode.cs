using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehavioursRectangularGraph
{
    public class RectangularNode<T> where T : RectangularNodeBehavior
    {
        public T SpawnedBehaviour { get; set; }
        public readonly T ReferenceBehaviour;

        public readonly RectangularNode<T>[] LeftNeighbours;
        public readonly RectangularNode<T>[] RightNeighbours;
        public readonly RectangularNode<T>[] TopNeighbours;
        public readonly RectangularNode<T>[] BottomNeighbours;

        public readonly int Depth;

        public Vector3 NodeWorldPosition { get; private set; }
        public bool IsDeadEnd => Depth > 0 &&
            (LeftNeighbours.Length + RightNeighbours.Length + TopNeighbours.Length + BottomNeighbours.Length) == 1;


        public RectangularNode(T behaviour, int depth)
        {
            Depth = depth;
            ReferenceBehaviour = behaviour;

            LeftNeighbours =
                new RectangularNode<T>[behaviour.GetExitsByDirection(RectangularDirection.Left).Count];
            RightNeighbours =
                new RectangularNode<T>[behaviour.GetExitsByDirection(RectangularDirection.Right).Count];
            TopNeighbours =
                new RectangularNode<T>[behaviour.GetExitsByDirection(RectangularDirection.Up).Count];
            BottomNeighbours =
                new RectangularNode<T>[behaviour.GetExitsByDirection(RectangularDirection.Down).Count];
        }

        public Vector3 GetExitWorldPosition(
            RectangularDirection exitDirection,
            int exitIndex)
        {
            return ReferenceBehaviour.GetExitWorldPosition(exitDirection, exitIndex, NodeWorldPosition);
        }

        public IEnumerable<RectangularNode<T>> GetAllCreatedNeighbours()
        {
            foreach (var direction in Utility.GetEachDirection())
            {
                foreach (var node in GetNeigboursByDirection(direction))
                {
                    if (node != null && node.Depth > Depth)
                    {
                        foreach (var innerCreatedNode in node.GetAllCreatedNeighbours())
                        {
                            yield return innerCreatedNode;
                        }

                        yield return node;
                    }
                }
            }
        }

        public RectangularNode<T>[] GetNeigboursByDirection(RectangularDirection direction)
        {
            return direction switch
            {
                RectangularDirection.Left => LeftNeighbours,
                RectangularDirection.Up => TopNeighbours,
                RectangularDirection.Right => RightNeighbours,
                RectangularDirection.Down => BottomNeighbours,
                _ => throw new System.NotImplementedException()
            };
        }

        public int SetAsRandomNeighbour(
            RectangularNode<T> neighbour,
            RectangularDirection neighbourDirection)
        {
            var directionNeighbours = GetNeigboursByDirection(neighbourDirection);
            var randomNeighbourIndex = Random.Range(0, directionNeighbours.Length);

            SetAsNeighbour(neighbour, neighbourDirection, randomNeighbourIndex);

            return randomNeighbourIndex;
        }

        public void SetAsNeighbour(
            RectangularNode<T> neighbour,
            RectangularDirection neighbourDirection,
            int neighbourIndex)
        {
            var directionNeighbours = GetNeigboursByDirection(neighbourDirection);
            directionNeighbours[neighbourIndex] = neighbour;
        }

        public void SetPositionRelatively(
            RectangularNode<T> neighbour,
            RectangularDirection neighbourDirection,
            int neighbourIndex,
            int neighbourExitIndex)
        {
            var neighbourExitWorldPosition = neighbour
                .GetExitWorldPosition(Utility.GetInversedDirection(neighbourDirection), neighbourExitIndex);

            NodeWorldPosition = ReferenceBehaviour.GetWorldPositionRelatively(
                neighbourDirection,
                neighbourIndex,
                neighbourExitWorldPosition
                );
        }

        public bool CheckGlobalCompatability(
            IEnumerable<RectangularNode<T>> otherNodes,
            RectangularNode<T> ignoreNode,
            out List<(RectangularNode<T> otherNode, List<Utility.CycleCheckInfo> cycleChecks)> foundCycles)
        {
            foundCycles = new List<(RectangularNode<T> otherNode, List<Utility.CycleCheckInfo> cycleChecks)>();
            foreach (var node in otherNodes)
            {
                if (node == ignoreNode)
                {
                    continue;
                }

                if (ReferenceBehaviour.TryGetCycles(
                        NodeWorldPosition,
                        node.ReferenceBehaviour,
                        node.NodeWorldPosition,
                        out var cycles
                        ))
                {
                    foundCycles.Add((node, cycles));
                    continue;
                }

                if (!ReferenceBehaviour.IsCompatible(
                        NodeWorldPosition,
                        node.ReferenceBehaviour,
                        node.NodeWorldPosition
                        ))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckGlobalCompatability(
            IEnumerable<RectangularNode<T>> otherNodes,
            RectangularNode<T> ignoreNode
            )
        {
            foreach (var node in otherNodes)
            {
                if (node == ignoreNode)
                {
                    continue;
                }

                if (!ReferenceBehaviour.IsCompatible(
                        NodeWorldPosition,
                        node.ReferenceBehaviour,
                        node.NodeWorldPosition
                        ))
                {
                    return false;
                }
            }

            return true;
        }
    }
}