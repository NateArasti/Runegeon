using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphs;
using UnityEngine;
using UnityExtensions;

namespace BehavioursRectangularGraph
{
    public class RectangularGraph<T> where T : RectangularNodeBehavior
    {
        public class SequenceGraphNodeBehaviourData : SpecialGraphNodeBehaviourData
        {
            public SequenceGraphNodeBehaviourData(
                T[] nextRoomVariations, 
                T[] behaviourVariations, 
                float spawnChance, 
                Vector2Int spawnRange) :
                base(behaviourVariations, spawnChance, spawnRange)
            {
                NextBehaviourVariations = nextRoomVariations;
            }

            public IReadOnlyCollection<T> NextBehaviourVariations { get; }
        }

        public class SpecialGraphNodeBehaviourData : GraphNodeBehaviourData
        {
            public SpecialGraphNodeBehaviourData(
                T[] behaviourVariations, 
                float spawnChance, 
                Vector2Int spawnRange) :
                base(behaviourVariations[0], spawnChance, spawnRange)
            {
                BehaviourVariations = behaviourVariations;
            }

            public IReadOnlyCollection<T> BehaviourVariations { get; }
        }

        public class GraphNodeBehaviourData
        {
            public GraphNodeBehaviourData(
                T behaviour, 
                float spawnChance, 
                Vector2Int spawnRange)
            {
                Behaviour = behaviour;
                SpawnChance = Mathf.Clamp01(spawnChance);
                SpawnRange = spawnRange;
            }

            public T Behaviour { get; }
            public float SpawnChance { get; }
            public Vector2Int SpawnRange { get; }
        }

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
        private readonly HashSet<SpecialGraphNodeBehaviourData> m_SpecialDatas;
        private readonly Dictionary<T, GraphNodeBehaviourData> m_BehaviourDatas;

        public HashSet<RectangularNode<T>> Nodes { get; } = new();
        public HashSet<SpecialGraphNodeBehaviourData> SpawnedSpecials { get; } = new();

        public Vector2Int DepthRange { get; set; } = new Vector2Int(2, 5);
        public float DeadEndChance { get; set; } = 0.1f;
        public bool HandleCycles { get; set; } = true;

        public RectangularGraph(
            IReadOnlyCollection<GraphNodeBehaviourData> possibleNodeBehaviourDatas, 
            IReadOnlyCollection<SpecialGraphNodeBehaviourData> specialNodeBehaviourDatas
            )
        {
            m_BehaviourDatas = new Dictionary<T, GraphNodeBehaviourData>();
            foreach (var data in possibleNodeBehaviourDatas)
            {
                m_BehaviourDatas.Add(data.Behaviour, data);
            }
            foreach (var data in specialNodeBehaviourDatas)
            {
                foreach (var variant in data.BehaviourVariations)
                {
                    m_BehaviourDatas.Add(variant, data);
                }
            }
            m_PossibleNodeBehaviours = new HashSet<T>(m_BehaviourDatas.Keys);
            m_SpecialDatas = new HashSet<SpecialGraphNodeBehaviourData>(specialNodeBehaviourDatas);
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
                .GetWeightedShuffle(GetBehaviourSpawnChance)
                .Where(behaviour =>
                    !m_BehaviourDatas.ContainsKey(behaviour) ||
                    Utility.InRange(0, m_BehaviourDatas[behaviour].SpawnRange));

            foreach(var startNodeBehaviour in possibleStartNodeBehaviours)
            {
                Nodes.Clear();
                SpawnedSpecials.Clear();
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
                .Where(behaviour =>
                    behaviour.RightExits.Count > 0 &&
                    (behaviour.LeftExits.Count != 0 ||
                    behaviour.TopExits.Count != 0 ||
                    behaviour.BottomExits.Count != 0)
                )
                .ToList();
            m_PossibleRightExitСontinuation = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.LeftExits.Count > 0 &&
                    (behaviour.RightExits.Count != 0 ||
                    behaviour.TopExits.Count != 0 ||
                    behaviour.BottomExits.Count != 0)
                )
                .ToList();
            m_PossibleTopExitСontinuation = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.BottomExits.Count > 0 &&
                    (behaviour.LeftExits.Count != 0 ||
                    behaviour.TopExits.Count != 0 ||
                    behaviour.RightExits.Count != 0)
                )
                .ToList();
            m_PossibleBottomExitСontinuation = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.TopExits.Count > 0 &&
                    (behaviour.LeftExits.Count != 0 ||
                    behaviour.RightExits.Count != 0 ||
                    behaviour.BottomExits.Count != 0)
                )
                .ToList();

            #endregion

            #region DeadEnds

            m_PossibleLeftDeadEnds = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.RightExits.Count == 1 &&
                    behaviour.LeftExits.Count == 0 &&
                    behaviour.TopExits.Count == 0 &&
                    behaviour.BottomExits.Count == 0)
                .ToList();
            m_PossibleRightDeadEnds = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.LeftExits.Count == 1 &&
                    behaviour.RightExits.Count == 0 &&
                    behaviour.TopExits.Count == 0 &&
                    behaviour.BottomExits.Count == 0)
                .ToList();
            m_PossibleTopDeadEnds = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.BottomExits.Count == 1 &&
                    behaviour.LeftExits.Count == 0 &&
                    behaviour.TopExits.Count == 0 &&
                    behaviour.RightExits.Count == 0)
                .ToList();
            m_PossibleBottomDeadEnds = m_PossibleNodeBehaviours
                .Where(behaviour =>
                    behaviour.TopExits.Count == 1 &&
                    behaviour.LeftExits.Count == 0 &&
                    behaviour.RightExits.Count == 0 &&
                    behaviour.BottomExits.Count == 0)
                .ToList();

            #endregion
        }

        private bool CreateNextNode(RectangularNode<T> node)
        {
            Nodes.Add(node);
            if(m_BehaviourDatas.ContainsKey(node.ReferenceBehaviour) &&
                m_BehaviourDatas[node.ReferenceBehaviour] is SpecialGraphNodeBehaviourData specialData)
            {
                SpawnedSpecials.Add(specialData);
            }

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

            if (m_BehaviourDatas.ContainsKey(node.ReferenceBehaviour) &&
                m_BehaviourDatas[node.ReferenceBehaviour] is SpecialGraphNodeBehaviourData specialData &&
                SpawnedSpecials.Contains(specialData))
            {
                SpawnedSpecials.Remove(specialData);
            }

            foreach (var neighbourNode in node.GetAllCreatedNeighbours())
            {
                Nodes.Remove(neighbourNode);
                if (m_BehaviourDatas.ContainsKey(neighbourNode.ReferenceBehaviour) &&
                    m_BehaviourDatas[neighbourNode.ReferenceBehaviour] is SpecialGraphNodeBehaviourData nodeSpecialData &&
                    SpawnedSpecials.Contains(nodeSpecialData))
                {
                    SpawnedSpecials.Remove(nodeSpecialData);
                }
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
                spawnDeadEnd,
                node.Depth + 1);

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
            var graphDepth = 0;
            foreach (var node in Nodes)
            {
                foreach(var direction in Utility.GetEachDirection())
                {
                    foreach (var neighbour in node.GetNeigboursByDirection(direction))
                    {
                        if(neighbour == null)
                        {
                            //graph not completed, no need to check further
                            return true;
                        }
                    }
                }

                graphDepth = Mathf.Max(graphDepth, node.Depth);
            }

            return Utility.InRange(graphDepth, DepthRange) && 
                SpawnedSpecials.Count == m_SpecialDatas.Count;
        }

        private IEnumerable<T> GetPossibleNextNodeBehaviours(
            T nodeBehaviour,
            RectangularDirection neighbourDirection, 
            bool spawnDeadEnd,
            int depth)
        {
            if (m_BehaviourDatas[nodeBehaviour] is SequenceGraphNodeBehaviourData sequence)
            {
                return neighbourDirection switch
                {
                    RectangularDirection.Left => sequence.NextBehaviourVariations.Where(behaviour =>
                        behaviour.RightExits.Count == 1 &&
                        behaviour.LeftExits.Count == 0 &&
                        behaviour.TopExits.Count == 0 &&
                        behaviour.BottomExits.Count == 0),
                    RectangularDirection.Up => sequence.NextBehaviourVariations.Where(behaviour =>
                        behaviour.BottomExits.Count == 1 &&
                        behaviour.LeftExits.Count == 0 &&
                        behaviour.TopExits.Count == 0 &&
                        behaviour.RightExits.Count == 0),
                    RectangularDirection.Right => sequence.NextBehaviourVariations.Where(behaviour =>
                        behaviour.LeftExits.Count == 1 &&
                        behaviour.RightExits.Count == 0 &&
                        behaviour.TopExits.Count == 0 &&
                        behaviour.BottomExits.Count == 0),
                    RectangularDirection.Down => sequence.NextBehaviourVariations.Where(behaviour =>
                        behaviour.TopExits.Count == 1 &&
                        behaviour.LeftExits.Count == 0 &&
                        behaviour.RightExits.Count == 0 &&
                        behaviour.BottomExits.Count == 0),
                    _ => null,
                };
            }

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
                .ToHashSet();

            foreach (var specialData in m_SpecialDatas)
            {
                if (!SpawnedSpecials.Contains(specialData)) continue;
                foreach (var behaviour in specialData.BehaviourVariations)
                {
                    if (shuffled.Contains(behaviour))
                    {
                        shuffled.Remove(behaviour);
                    }
                }
            }

            //to prevent placing same room twice
            if (shuffled.Contains(nodeBehaviour))
            {
                shuffled.Remove(nodeBehaviour);
                shuffled.Add(nodeBehaviour);
            }

            return shuffled.Where(behaviour => 
                    !m_BehaviourDatas.ContainsKey(behaviour) ||
                    Utility.InRange(depth, m_BehaviourDatas[behaviour].SpawnRange)
                    );
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
            var spawnChance = !m_BehaviourDatas.ContainsKey(behaviour) || m_BehaviourDatas[behaviour] is SpecialGraphNodeBehaviourData 
                ? 1 
                : m_BehaviourDatas[behaviour].SpawnChance;
            return spawnChance * 100;
        }
    }
}