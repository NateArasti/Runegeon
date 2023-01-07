using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityExtensions;

public class LocationGenerator : MonoBehaviour
{
    [Space]
    [SerializeField] private UnityEvent<RectangularNode<Room>> m_OnLocationGenerated;
    [Space]
    [SerializeField] private List<Room> m_RoomsPrefabs;

    [BoxGroup("Random"), SerializeField] private bool m_CustomSeed;
    [BoxGroup("Random"), SerializeField, ShowIf(nameof(m_CustomSeed))] private int m_Seed;

    [BoxGroup("Generation Params"), SerializeField] private bool m_GenerateOnAwake = true;
    [BoxGroup("Generation Params"), SerializeField, MinValue(2), MaxValue(10)] private int m_MaxDepth = 5;
    [BoxGroup("Generation Params"), SerializeField, Range(0, 1)] private float m_DeadEndChance = 0.1f;
    [BoxGroup("Generation Params"), SerializeField] private bool m_HandleCycles = true;
    [BoxGroup("Generation Params"), SerializeField] private bool m_ColorRoomDueToDepth = true;
    [BoxGroup("Generation Params"), SerializeField, ShowIf(nameof(m_ColorRoomDueToDepth))] 
    private Gradient m_DepthGradient;

    private readonly HashSet<Room> SpawnedRooms = new();

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
        if(m_RoomsPrefabs.Count < 2)
        {
            Debug.LogError($"Not enough room prefabs! Must have at least 2, but was {m_RoomsPrefabs.Count}");
            return;
        }

#if UNITY_EDITOR
        if(Application.isPlaying)
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
        SpawnedRooms.Clear();

        var graph = new RectangularGraph<Room>(m_RoomsPrefabs)
        {
            DeadEndChance = m_DeadEndChance,
            MaxDepth = m_MaxDepth,
            HandleCycles = m_HandleCycles,
        };

        var success = graph.TryGenerateNodes();
        if (success)
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
                    room.SetRoomColor(m_DepthGradient.Evaluate((float)node.Depth / m_MaxDepth));

                node.SpawnedBehaviour = room;
                SpawnedRooms.Add(room);
            }
        }
        var result = success ? "success" : "fail";
        Debug.Log($"Generation finished with result - {result}");

        if (success)
        {
            var startNode = graph.Nodes.First();
            m_OnLocationGenerated.Invoke(startNode);
        }
    }
}
