using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityExtensions;

public class LocationGenerator : MonoBehaviour
{
    [Space]
    [SerializeField] private UnityEvent<RectangularNode<Room>> m_OnLocationGenerated;
    [Space]
    [SerializeField] private List<CommonGraphNodeBehaviourDataSerialized> m_CommonRoomsPrefabs;
    [SerializeField] private List<SpecialGraphNodeBehaviourDataSerialized> m_SpecialRoomsPrefabs;
    [SerializeField] private List<SequenceGraphNodeBehaviourDataSerialized> m_SpecialSequenceRoomsPrefabs;

    [BoxGroup("Random"), SerializeField] private bool m_CustomSeed;
    [BoxGroup("Random"), SerializeField, ShowIf(nameof(m_CustomSeed))] private int m_Seed;

    [BoxGroup("Generation Params"), SerializeField] private bool m_GenerateOnAwake = true;
    [BoxGroup("Generation Params"), SerializeField, MinMaxSlider(1, 20)] private Vector2Int m_DepthRange = new Vector2Int(2, 5);
    [BoxGroup("Generation Params"), SerializeField, Range(0, 1)] private float m_DeadEndChance = 0.1f;
    [BoxGroup("Generation Params"), SerializeField] private bool m_HandleCycles = true;
    [BoxGroup("Generation Params"), SerializeField] private bool m_ColorRoomDueToDepth = true;
    [BoxGroup("Generation Params"), SerializeField, ShowIf(nameof(m_ColorRoomDueToDepth))] private Gradient m_DepthGradient;
    
    [Foldout("Analyse Params"), SerializeField, Range(1, 1000)] private int m_SimulationCount = 100;

    private readonly HashSet<Room> m_SpawnedRooms = new();

    public IReadOnlyCollection<RectangularNode<Room>> RoomNodes { get; private set; }

    private void Awake()
    {
        if (!m_CustomSeed)
        {
            m_Seed = System.BitConverter.ToInt32(System.Guid.NewGuid().ToByteArray());
        }
        Random.InitState(m_Seed);
    }

    private void Start()
    {
        if (m_GenerateOnAwake)
            Generate();
    }

    [Button("Generate")]
    public void Generate()
    {
        PrepareGeneration();

        var commonRoomDatas = m_CommonRoomsPrefabs.Select(data => data.GraphNodeBehaviourData).ToArray();
        var specialRoomDatas = m_SpecialRoomsPrefabs.Select(data => data.GraphNodeBehaviourData)
            .Concat(m_SpecialSequenceRoomsPrefabs.Select(data => data.GraphNodeBehaviourData))
            .ToArray();

        var graph = new RectangularGraph<Room>(commonRoomDatas, specialRoomDatas)
        {
            DeadEndChance = m_DeadEndChance,
            DepthRange = m_DepthRange,
            HandleCycles = m_HandleCycles,
        };
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        var generationSuccess = graph.TryGenerateNodes();
        stopwatch.Stop();

        if (generationSuccess)
            SpawnLocation(graph);

        var result = generationSuccess ? "success" : "fail";
        Debug.Log($"Generation finished with result - {result}, time - {stopwatch.ElapsedMilliseconds} ms");
    }

    private void PrepareGeneration()
    {
        if (m_CommonRoomsPrefabs.Count < 2)
        {
            Debug.LogError($"Not enough room prefabs! Must have at least 2, but was {m_CommonRoomsPrefabs.Count}");
            return;
        }

#if UNITY_EDITOR
        if (Application.isPlaying)
            transform.DestroyChildren();
        else
        {
            Awake();

            var children = new List<GameObject>();
            foreach (Transform child in transform)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => DestroyImmediate(child));
        }
#else
        transform.DestroyChildren();
#endif
        m_SpawnedRooms.Clear();
    }

    private void SpawnLocation(RectangularGraph<Room> graph)
    {
        foreach (var node in graph.Nodes)
        {
            var room = Instantiate(
                node.ReferenceBehaviour,
                node.NodeWorldPosition,
                Quaternion.identity,
                transform
                );

            room.name = $"{node.ReferenceBehaviour.name}_[id = {System.Guid.NewGuid().ToString()[0..3]}]";
            room.Depth = node.Depth;

            //just for debug puprose
            if (m_ColorRoomDueToDepth)
                room.SetRoomColor(m_DepthGradient.Evaluate(node.Depth / m_DepthRange.y));

            node.SpawnedBehaviour = room;
            m_SpawnedRooms.Add(room);
        }

        var startNode = graph.Nodes.First();
        RoomNodes = graph.Nodes;
        m_OnLocationGenerated.Invoke(startNode);
    }

#if UNITY_EDITOR

    [SerializeField, Foldout("EditorOnly")] private Room[] m_RoomsToAdd;
    [SerializeField, Range(0, 1), Foldout("EditorOnly")] private float m_DefaultSpawnChance = 1;
    [SerializeField, MinMaxSlider(0, 100), Foldout("EditorOnly")] private Vector2Int m_DefaultSpawnRange = new Vector2Int(0, 100);

    [Button(enabledMode: EButtonEnableMode.Editor)]
    private void AddToCommonRunes()
    {
        foreach (var room in m_RoomsToAdd)
        {
            m_CommonRoomsPrefabs.Add(
                new CommonGraphNodeBehaviourDataSerialized(room, m_DefaultSpawnChance, m_DefaultSpawnRange)
                );
        }
    }


    [Button(enabledMode: EButtonEnableMode.Editor)]
    public void RunAnalysis()
    {
        var commonRoomDatas = m_CommonRoomsPrefabs.Select(data => data.GraphNodeBehaviourData).ToArray();
        var specialRoomDatas = m_SpecialRoomsPrefabs.Select(data => data.GraphNodeBehaviourData).ToArray();

        var graph = new RectangularGraph<Room>(commonRoomDatas, specialRoomDatas)
        {
            DeadEndChance = m_DeadEndChance,
            DepthRange = m_DepthRange,
            HandleCycles = m_HandleCycles,
        };

        var analyseLog = new StringBuilder();
        analyseLog.AppendLine("<b><size=15><color=red>Location generation analysis:</color></size></b>");

        var successfullGenerationsCount = 0;
        var totalCretedNodesCount = 0;
        var roomsStatistics = new Dictionary<Room, int>();

        foreach (var roomData in commonRoomDatas)
        {
            roomsStatistics.Add(roomData.Behaviour, 0);
        }

        for (int i = 0; i < m_SimulationCount; i++)
        {
            var generationSuccess = graph.TryGenerateNodes();

            if (generationSuccess)
            {
                successfullGenerationsCount++;

                foreach (var node in graph.Nodes)
                {
                    if (roomsStatistics.ContainsKey(node.ReferenceBehaviour))
                    {
                        roomsStatistics[node.ReferenceBehaviour]++;
                    }
                    else
                    {
                        Debug.LogError($"Unknown Room - {node.ReferenceBehaviour.gameObject.name}", node.ReferenceBehaviour);
                    }
                }

                totalCretedNodesCount += graph.Nodes.Count;
            }
        }

        analyseLog.AppendLine();
        analyseLog.AppendLine($"<i>Iterations count = {m_SimulationCount}</i>");
        analyseLog.AppendLine($"<i>Rooms Prefabs count = {commonRoomDatas.Length}</i>");
        analyseLog.AppendLine($"<i>Depth Range = [min:{m_DepthRange.x}, max:{m_DepthRange.y}]</i>");
        analyseLog.AppendLine($"<i>Handling Cycles = {m_HandleCycles}</i>");
        analyseLog.AppendLine($"<i>DeadEnd Chance = {m_DeadEndChance}</i>");
        analyseLog.AppendLine();

        foreach (var roomData in commonRoomDatas)
        {
            analyseLog.AppendLine($"<color=aqua>Room [{roomData.Behaviour.gameObject.name}]</color> stats: absolute usage count = <b>{roomsStatistics[roomData.Behaviour]}</b>, relative usage = <b>{(float)roomsStatistics[roomData.Behaviour] / totalCretedNodesCount}</b>");
        }

        Debug.Log(analyseLog.ToString());
    }

#endif
}
