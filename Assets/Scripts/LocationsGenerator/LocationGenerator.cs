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
    [SerializeField] private List<Room> m_RoomsPrefabs;
    [SerializeField] private List<Room> m_RequiredRooms;

    [BoxGroup("Random"), SerializeField] private bool m_CustomSeed;
    [BoxGroup("Random"), SerializeField, ShowIf(nameof(m_CustomSeed))] private int m_Seed;

    [BoxGroup("Generation Params"), SerializeField] private bool m_GenerateOnAwake = true;
    [BoxGroup("Generation Params"), SerializeField, MinMaxSlider(1, 20)] private Vector2 m_DepthRange = new Vector2(2, 5);
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

        var graph = new RectangularGraph<Room>(m_RoomsPrefabs, m_RequiredRooms)
        {
            DeadEndChance = m_DeadEndChance,
            DepthRange = m_DepthRange,
            HandleCycles = m_HandleCycles,
        };

        var generationSuccess = graph.TryGenerateNodes();

        if(generationSuccess)
            SpawnLocation(graph);

        var result = generationSuccess ? "success" : "fail";
        Debug.Log($"Generation finished with result - {result}");
    }

    private void PrepareGeneration()
    {
        if (m_RoomsPrefabs.Count < 2)
        {
            Debug.LogError($"Not enough room prefabs! Must have at least 2, but was {m_RoomsPrefabs.Count}");
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

    [Button(enabledMode: EButtonEnableMode.Editor)]
    public void RunAnalysis()
    {
        var graph = new RectangularGraph<Room>(m_RoomsPrefabs, m_RequiredRooms)
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

        foreach (var roomPrefab in m_RoomsPrefabs)
        {
            roomsStatistics.Add(roomPrefab, 0);
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
        analyseLog.AppendLine($"<i>Rooms Prefabs count = {m_RoomsPrefabs.Count}</i>");
        analyseLog.AppendLine($"<i>Depth Range = [min:{m_DepthRange.x}, max:{m_DepthRange.y}]</i>");
        analyseLog.AppendLine($"<i>Handling Cycles = {m_HandleCycles}</i>");
        analyseLog.AppendLine($"<i>DeadEnd Chance = {m_DeadEndChance}</i>");
        analyseLog.AppendLine();
        foreach (var roomPrefab in m_RoomsPrefabs)
        {
            analyseLog.AppendLine($"<color=aqua>Room [{roomPrefab.gameObject.name}]</color> stats: absolute usage count = <b>{roomsStatistics[roomPrefab]}</b>, relative usage = <b>{(float)roomsStatistics[roomPrefab] / totalCretedNodesCount}</b>");
        }

        Debug.Log(analyseLog.ToString());
    }

#endif
}
