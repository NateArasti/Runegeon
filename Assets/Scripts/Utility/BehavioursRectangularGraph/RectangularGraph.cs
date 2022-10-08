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

        private List<T> _possibleLeftExitСontinuation;
        private List<T> _possibleRightExitСontinuation;
        private List<T> _possibleTopExitСontinuation;
        private List<T> _possibleBottomExitСontinuation;

        #endregion

        #region DeadEnds

        private List<T> _possibleLeftDeadEnds;
        private List<T> _possibleRightDeadEnds;
        private List<T> _possibleTopDeadEnds;
        private List<T> _possibleBottomDeadEnds;

        #endregion

        #endregion

        public IReadOnlyList<T> PossibleNodeBehaviours;
        public readonly HashSet<RectangularNode<T>> Nodes = new();

        public RectangularGraph(IReadOnlyList<T> possibleNodeBehaviours)
        {
            PossibleNodeBehaviours = possibleNodeBehaviours;
        }

        public int MaxDepth { get; set; } = 5;
        public float DeadEndChance { get; set; } = 0.1f;
        public bool HandleCycles { get; set; } = true;

        public bool TryGenerateNodes()
        {
            if (PossibleNodeBehaviours == null || PossibleNodeBehaviours.Count == 0)
            {
                Debug.LogWarning("Node Behaviours Collection is null or empty");
                return false;
            }

            CalculateRoomPossiblities();

            var possibleStartNodeBehaviours = PossibleNodeBehaviours
                .Where(nodeBehaviour => nodeBehaviour.LeftExits.Count == 0)
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

            _possibleLeftExitСontinuation = PossibleNodeBehaviours
                .Where(room =>
                    room.RightExits.Count > 0 &&
                    (room.LeftExits.Count != 0 ||
                    room.TopExits.Count != 0 ||
                    room.BottomExits.Count != 0)
                )
                .ToList();
            _possibleRightExitСontinuation = PossibleNodeBehaviours
                .Where(room =>
                    room.LeftExits.Count > 0 &&
                    (room.RightExits.Count != 0 ||
                    room.TopExits.Count != 0 ||
                    room.BottomExits.Count != 0)
                )
                .ToList();
            _possibleTopExitСontinuation = PossibleNodeBehaviours
                .Where(room =>
                    room.BottomExits.Count > 0 &&
                    (room.LeftExits.Count != 0 ||
                    room.TopExits.Count != 0 ||
                    room.RightExits.Count != 0)
                )
                .ToList();
            _possibleBottomExitСontinuation = PossibleNodeBehaviours
                .Where(room =>
                    room.TopExits.Count > 0 &&
                    (room.LeftExits.Count != 0 ||
                    room.RightExits.Count != 0 ||
                    room.BottomExits.Count != 0)
                )
                .ToList();

            #endregion

            #region DeadEnds

            _possibleLeftDeadEnds = PossibleNodeBehaviours
                .Where(room =>
                    room.RightExits.Count > 0 &&
                    room.LeftExits.Count == 0 &&
                    room.TopExits.Count == 0 &&
                    room.BottomExits.Count == 0)
                .ToList();
            _possibleRightDeadEnds = PossibleNodeBehaviours
                .Where(room =>
                    room.LeftExits.Count > 0 &&
                    room.RightExits.Count == 0 &&
                    room.TopExits.Count == 0 &&
                    room.BottomExits.Count == 0)
                .ToList();
            _possibleTopDeadEnds = PossibleNodeBehaviours
                .Where(room =>
                    room.BottomExits.Count > 0 &&
                    room.LeftExits.Count == 0 &&
                    room.TopExits.Count == 0 &&
                    room.RightExits.Count == 0)
                .ToList();
            _possibleBottomDeadEnds = PossibleNodeBehaviours
                .Where(room =>
                    room.TopExits.Count > 0 &&
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
                if (!TryGetNextNode(node, neighbourDirection, neighbourIndex))
                {
                    Nodes.Remove(node);
                    foreach (var neighbourNode in node.GetAllCreatedNeighbours())
                    {
                        Nodes.Remove(neighbourNode);
                    }
                    return false;
                }
            }

            return true;
        }

        private bool TryGetNextNode(
            RectangularNode<T> node,
            RectangularDirection neighbourDirection,
            int neighbourIndex)
        {
            var spawnDeadEnd = node.Depth == MaxDepth - 1 || Random.value < DeadEndChance;

            var possibleNextNodeBehaviours = GetPossibleNextNodeBehaviours(neighbourDirection, spawnDeadEnd);

            foreach (var nodeBehaviour in possibleNextNodeBehaviours)
            {
                var newRoomNode = new RectangularNode<T>(nodeBehaviour, node.Depth + 1);
                node.GetNeigboursByDirection(neighbourDirection)[neighbourIndex] = newRoomNode;

                var exitWorldPosition = node.GetExitWorldPosition(neighbourDirection, neighbourIndex);
                newRoomNode.SetAsNeighbour(node, neighbourDirection, exitWorldPosition);

                if (newRoomNode.CheckGlobalCompatability(Nodes, node, HandleCycles) && CreateNextNode(newRoomNode))
                {
                    return true;
                }

                node.GetNeigboursByDirection(neighbourDirection)[neighbourIndex] = null;
            }

            return false;
        }

        private IEnumerable<T> GetPossibleNextNodeBehaviours(RectangularDirection neighbourDirection, bool spawnDeadEnd)
        {
            var possibleNextNodeBehaviours = (neighbourDirection, spawnDeadEnd) switch
            {
                (RectangularDirection.Left, true) => _possibleLeftDeadEnds,
                (RectangularDirection.Right, true) => _possibleRightDeadEnds,
                (RectangularDirection.Down, true) => _possibleBottomDeadEnds,
                (RectangularDirection.Up, true) => _possibleTopDeadEnds,
                (RectangularDirection.Left, false) => _possibleLeftExitСontinuation,
                (RectangularDirection.Right, false) => _possibleRightExitСontinuation,
                (RectangularDirection.Down, false) => _possibleBottomExitСontinuation,
                (RectangularDirection.Up, false) => _possibleTopExitСontinuation,
                _ => throw new System.NotImplementedException(),
            };

            return possibleNextNodeBehaviours.GetWeightedShuffle(behaviour => behaviour.SpawnChance);
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
    }
}