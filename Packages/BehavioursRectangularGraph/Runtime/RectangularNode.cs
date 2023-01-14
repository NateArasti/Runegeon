using System.Collections.Generic;
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

        public void SetAsNeighbour(
            RectangularNode<T> neighbour,
            RectangularDirection neighbourDirection,
            Vector3 neighbourExitWorldPosition)
        {
            var inverseDirection = Utility.GetInversedDirection(neighbourDirection);
            var directionNeighbours = GetNeigboursByDirection(inverseDirection);
            var neighbourIndex = Random.Range(0, directionNeighbours.Length);
            directionNeighbours[neighbourIndex] = neighbour;

            NodeWorldPosition = ReferenceBehaviour.GetWorldPositionRelatively(
                inverseDirection,
                neighbourIndex,
                neighbourExitWorldPosition
                );
        }

        public bool CheckGlobalCompatability(
            IEnumerable<RectangularNode<T>> otherNodes,
            RectangularNode<T> ignoreNode,
            bool handleCycles = true)
        {
            var foundCycles = new 
                List<(RectangularNode<T> otherNode, List<Utility.CycleCheckInfo> cycleChecks)>();
            foreach (var node in otherNodes)
            {
                if (node == ignoreNode)
                {
                    continue;
                }
                
                if(handleCycles && ReferenceBehaviour.TryGetCycles(
                        NodeWorldPosition, 
                        node.ReferenceBehaviour,
                        node.NodeWorldPosition,
                        out var cycles
                        )
                    )
                {
                    foundCycles.Add((node, cycles));
                    continue;
                }

                if (!ReferenceBehaviour.IsCompatible(
                        NodeWorldPosition,
                        node.ReferenceBehaviour,
                        node.NodeWorldPosition
                        )
                    )
                {
                    return false;
                }
            }

            foreach (var cycle in foundCycles)
            {
                foreach (var cycleCheck in cycle.cycleChecks)
                {
                    GetNeigboursByDirection(cycleCheck.direction)[cycleCheck.exitIndex] = cycle.otherNode;
                    cycle.otherNode
                        .GetNeigboursByDirection(cycleCheck.otherDirection)
                        [cycleCheck.otherExitIndex] = this;
                }
            }

            return true;
        }
    }
}