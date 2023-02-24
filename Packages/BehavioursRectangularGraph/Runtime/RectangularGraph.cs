using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtensions;

namespace BehavioursRectangularGraph
{
    public class RectangularGraph<T> where T : RectangularNodeBehavior
    {
        #region Precalculated Possible Rooms

        #region SimpleContinueRooms

        private List<T> m_PossibleLeftExitСontinuation;
        private List<T> m_PossibleRightExitСontinuation;
        private List<T> m_PossibleTopExitСontinuation;
        private List<T> m_PossibleBottomExitСontinuation;

        #endregion

        #region DeadEnds

        private List<T> m_PossibleLeftDeadEnds;
        private List<T> m_PossibleRightDeadEnds;
        private List<T> m_PossibleTopDeadEnds;
        private List<T> m_PossibleBottomDeadEnds;

        #endregion

        #endregion

        private readonly HashSet<T> m_PossibleNodeBehaviours;
        private readonly HashSet<T> m_RequiredNodeBehaviours;

        public HashSet<RectangularNode<T>> Nodes { get; } = new();

        public Vector2 DepthRange { get; set; } = new Vector2(2, 5);
        public float DeadEndChance { get; set; } = 0.1f;
        public bool HandleCycles { get; set; } = true;

        public RectangularGraph(IReadOnlyCollection<T> possibleNodeBehaviours, IReadOnlyCollection<T> requiredNodeBehaviours = null)
        {
            var allPossibleBehaviours = new HashSet<T>(possibleNodeBehaviours);
            var allRequiredBehaviours = requiredNodeBehaviours != null ? 
                new HashSet<T>(requiredNodeBehaviours) : new HashSet<T>();
            foreach (var requiredBehaviour in allRequiredBehaviours)
            {
                allPossibleBehaviours.Add(requiredBehaviour);
            }
            m_PossibleNodeBehaviours = allPossibleBehaviours;
            m_RequiredNodeBehaviours = allRequiredBehaviours;
        }

        public bool TryGenerateNodes()
        {
            if (m_PossibleNodeBehaviours == null || m_PossibleNodeBehaviours.Count < 2)
            {
                Debug.LogWarning("Node Behaviours Collection is null or doesn't have enough node behaviours");
                return false;
            }

            CalculateRoomPossiblities();

            var possibleStartNodeBehaviours = m_PossibleNodeBehaviours
                //.Where(nodeBehaviour => nodeBehaviour.LeftExits.Count == 0)
                .GetWeightedShuffle(behaviour => behaviour.SpawnChance);

            foreach(var startNodeBehaviour in possibleStartNodeBehaviours)
            {
                Nodes.Clear();
                var startNode = new RectangularNode<T>(startNodeBehaviour, 0);

                if (CreateNextNode(startNode))
                {
                    return true;
                }
            }

            return false;
        }

        private void CalculateRoomPossiblities()
        {
            #region SimpleContinueRooms

            m_PossibleLeftExitСontinuation = m_PossibleNodeBehaviours
                .Where(room =>
                    room.RightExits.Count > 0 &&
                    (room.LeftExits.Count != 0 ||
                    room.TopExits.Count != 0 ||
                    room.BottomExits.Count != 0)
                )
                .ToList();
            m_PossibleRightExitСontinuation = m_PossibleNodeBehaviours
                .Where(room =>
                    room.LeftExits.Count > 0 &&
                    (room.RightExits.Count != 0 ||
                    room.TopExits.Count != 0 ||
                    room.BottomExits.Count != 0)
                )
                .ToList();
            m_PossibleTopExitСontinuation = m_PossibleNodeBehaviours
                .Where(room =>
                    room.BottomExits.Count > 0 &&
                    (room.LeftExits.Count != 0 ||
                    room.TopExits.Count != 0 ||
                    room.RightExits.Count != 0)
                )
                .ToList();
            m_PossibleBottomExitСontinuation = m_PossibleNodeBehaviours
                .Where(room =>
                    room.TopExits.Count > 0 &&
                    (room.LeftExits.Count != 0 ||
                    room.RightExits.Count != 0 ||
                    room.BottomExits.Count != 0)
                )
                .ToList();

            #endregion

            #region DeadEnds

            m_PossibleLeftDeadEnds = m_PossibleNodeBehaviours
                .Where(room =>
                    room.RightExits.Count == 1 &&
                    room.LeftExits.Count == 0 &&
                    room.TopExits.Count == 0 &&
                    room.BottomExits.Count == 0)
                .ToList();
            m_PossibleRightDeadEnds = m_PossibleNodeBehaviours
                .Where(room =>
                    room.LeftExits.Count == 1 &&
                    room.RightExits.Count == 0 &&
                    room.TopExits.Count == 0 &&
                    room.BottomExits.Count == 0)
                .ToList();
            m_PossibleTopDeadEnds = m_PossibleNodeBehaviours
                .Where(room =>
                    room.BottomExits.Count == 1 &&
                    room.LeftExits.Count == 0 &&
                    room.TopExits.Count == 0 &&
                    room.RightExits.Count == 0)
                .ToList();
            m_PossibleBottomDeadEnds = m_PossibleNodeBehaviours
                .Where(room =>
                    room.TopExits.Count == 1 &&
                    room.LeftExits.Count == 0 &&
                    room.RightExits.Count == 0 &&
                    room.BottomExits.Count == 0)
                .ToList();

            #endregion
        }

        private bool CreateNextNode(RectangularNode<T> node)
        {
            Nodes.Add(node);

            var requirableNeighbours = GetShuffledRequirableNeighbours(node);

            for (var i = 0; i < requirableNeighbours.Count; ++i)
            {
                var (neighbourDirection, neighbourIndex) = requirableNeighbours[i];
                // Skipping neighbours setted by cycle
                if (node.GetNeigboursByDirection(neighbourDirection)[neighbourIndex] != null) continue;
                if (!TryGetNextNode(node, neighbourDirection, neighbourIndex))
                {
                    WipeNodeOutOfGraph(node);
                    return false;
                }
            }

            if (!CheckGraphConsistancy())
            {
                WipeNodeOutOfGraph(node);
                return false;
            }

            return true;
        }

        private void WipeNodeOutOfGraph(RectangularNode<T> node)
        {
            Nodes.Remove(node);
            foreach (var neighbourNode in node.GetAllCreatedNeighbours())
            {
                Nodes.Remove(neighbourNode);
            }
        }

        private bool TryGetNextNode(
            RectangularNode<T> node,
            RectangularDirection neighbourDirection,
            int neighbourIndex)
        {
            if (node.Depth >= DepthRange.y) return false;

            var spawnDeadEnd = node.Depth == DepthRange.y - 1 || Random.value < DeadEndChance;

            var possibleNextNodeBehaviours = GetPossibleNextNodeBehaviours(
                node.ReferenceBehaviour, 
                neighbourDirection, 
                spawnDeadEnd);

            foreach (var nodeBehaviour in possibleNextNodeBehaviours)
            {
                var newRoomNode = new RectangularNode<T>(nodeBehaviour, node.Depth + 1);

                node.SetAsNeighbour(newRoomNode, neighbourDirection, neighbourIndex);
                var newRoomNodeDirection = Utility.GetInversedDirection(neighbourDirection);
                var newNeighbourIndex = newRoomNode.SetAsRandomNeighbour(node, newRoomNodeDirection);
                newRoomNode.SetPositionRelatively(node, newRoomNodeDirection, newNeighbourIndex, neighbourIndex);

                if (HandleCycles && newRoomNode.CheckGlobalCompatability(Nodes, node, out var foundCycles))
                {
                    foreach (var cycle in foundCycles)
                    {
                        var otherNode = cycle.otherNode;
                        foreach (var cycleCheck in cycle.cycleChecks)
                        {
                            newRoomNode.SetAsNeighbour(cycle.otherNode, cycleCheck.direction, cycleCheck.exitIndex);
                            otherNode.SetAsNeighbour(newRoomNode, cycleCheck.otherDirection, cycleCheck.otherExitIndex);
                        }
                    }

                    if (CreateNextNode(newRoomNode))
                    {
                        return true;
                    }

                    foreach (var cycle in foundCycles)
                    {
                        var otherNode = cycle.otherNode;
                        foreach (var cycleCheck in cycle.cycleChecks)
                        {
                            newRoomNode.SetAsNeighbour(null, cycleCheck.direction, cycleCheck.exitIndex);
                            otherNode.SetAsNeighbour(null, cycleCheck.otherDirection, cycleCheck.otherExitIndex);
                        }
                    }
                }
                else if (newRoomNode.CheckGlobalCompatability(Nodes, node) && CreateNextNode(newRoomNode))
                {
                    return true;
                }

                node.SetAsNeighbour(null, neighbourDirection, neighbourIndex);
            }

            return false;
        }

        private bool CheckGraphConsistancy()
        {
            var graphCompleted = true;
            var graphDepth = 0;
            var createdRequiredBehaviours = new HashSet<T>();
            foreach (var node in Nodes)
            {
                foreach(var direction in Utility.GetEachDirection())
                {
                    foreach (var neighbour in node.GetNeigboursByDirection(direction))
                    {
                        if(neighbour == null)
                        {
                            graphCompleted = false;
                            return true;
                        }
                    }
                }

                graphDepth = Mathf.Max(graphDepth, node.Depth);

                if (m_RequiredNodeBehaviours.Contains(node.ReferenceBehaviour) &&
                    !createdRequiredBehaviours.Contains(node.ReferenceBehaviour))
                {
                    createdRequiredBehaviours.Add(node.ReferenceBehaviour);
                }
            }

            return DepthRange.x <= graphDepth && graphDepth <= DepthRange.y &&
                createdRequiredBehaviours.Count == m_RequiredNodeBehaviours.Count;
        }

        private IEnumerable<T> GetPossibleNextNodeBehaviours(
            T nodeBehaviour,
            RectangularDirection neighbourDirection, 
            bool spawnDeadEnd)
        {
            var possibleNextNodeBehaviours = (neighbourDirection, spawnDeadEnd) switch
            {
                (RectangularDirection.Left, true) => m_PossibleLeftDeadEnds,
                (RectangularDirection.Right, true) => m_PossibleRightDeadEnds,
                (RectangularDirection.Down, true) => m_PossibleBottomDeadEnds,
                (RectangularDirection.Up, true) => m_PossibleTopDeadEnds,
                (RectangularDirection.Left, false) => m_PossibleLeftExitСontinuation,
                (RectangularDirection.Right, false) => m_PossibleRightExitСontinuation,
                (RectangularDirection.Down, false) => m_PossibleBottomExitСontinuation,
                (RectangularDirection.Up, false) => m_PossibleTopExitСontinuation,
                _ => throw new System.NotImplementedException(),
            };

            var shuffled = possibleNextNodeBehaviours
                .GetWeightedShuffle(GetBehaviourSpawnChance)
                .ToList();
            if (shuffled.Contains(nodeBehaviour))
            {
                shuffled.Remove(nodeBehaviour);
                shuffled.Add(nodeBehaviour);
            }
            return shuffled;
        }

        private List<(RectangularDirection neighbourDirection, int neighbourIndex)>
            GetShuffledRequirableNeighbours(RectangularNode<T> node)
        {
            var requirableNeighbours = new List<(RectangularDirection, int)>();

            foreach (var direction in Utility.GetEachDirection())
            {
                var neighboursAtDirection = node.GetNeigboursByDirection(direction);
                for (int i = 0; i < neighboursAtDirection.Length; i++)
                {
                    if (neighboursAtDirection[i] == null)
                    {
                        requirableNeighbours.Add((direction, i));
                    }
                }
            }

            requirableNeighbours.Shuffle();

            return requirableNeighbours;
        }

        private float GetBehaviourSpawnChance(T behaviour)
        {
            var spawnChance = m_RequiredNodeBehaviours.Contains(behaviour) ? 1 : behaviour.SpawnChance;
            return spawnChance * 100;
        }
    }
}