using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityExtensions;

public class LocationGenerator : MonoBehaviour
{
    [SerializeField] private List<Room> _roomsPrefabs;
    [SerializeField] private MapGenerator _mapGenerator;

    [BoxGroup("Random"), SerializeField] private bool _customSeed;
    [BoxGroup("Random"), SerializeField, ShowIf(nameof(_customSeed))] private int _seed;

    [BoxGroup("Generation Params"), SerializeField, MinValue(2), MaxValue(10)] private int _maxDepth = 5;
    [BoxGroup("Generation Params"), SerializeField, Range(0, 1)] private float _deadEndChance = 0.1f;
    [BoxGroup("Generation Params"), SerializeField] private bool _handleCycles = true;
    [BoxGroup("Generation Params"), SerializeField] private bool _colorRoomDueToDepth = true;
    [BoxGroup("Generation Params"), SerializeField, ShowIf(nameof(_colorRoomDueToDepth))] private Gradient _depthGradient;

    private readonly HashSet<Room> _spawnedRooms = new();

    private void Awake()
    {
        if (!_customSeed)
        {
            _seed = System.BitConverter.ToInt32(System.Guid.NewGuid().ToByteArray());
        }
        Random.InitState(_seed);
    }

    [Button("Generate")]
    public void Generate()
    {
        if(_roomsPrefabs.Count < 2)
        {
            Debug.LogError($"Not enough room prefabs! Must have at least 2, but was {_roomsPrefabs.Count}");
            return;
        }

#if UNITY_EDITOR
        if(EditorApplication.isPlaying)
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
        _spawnedRooms.Clear();

        var graph = new RectangularGraph<Room>(_roomsPrefabs)
        {
            DeadEndChance = _deadEndChance,
            MaxDepth = _maxDepth,
            HandleCycles = _handleCycles,
        };

        var success = graph.TryGenerateNodes();
        if (success)
        {
            foreach (var node in graph.Nodes)
            {
                var room = Object.Instantiate(
                    node.ReferenceBehaviour,
                    node.NodeWorldPosition,
                    Quaternion.identity,
                    transform
                    );

                room.name = $"{node.ReferenceBehaviour.name}_[id = {System.Guid.NewGuid().ToString()[0..3]}]";

                //just for debug puprose
                if(_colorRoomDueToDepth)
                    room.SetRoomColor(_depthGradient.Evaluate((float)node.Depth / _maxDepth));

                _spawnedRooms.Add(room);
            }
        }
        var result = success ? "success" : "fail";
        Debug.Log($"Generation finished with result - {result}");

        if(_mapGenerator != null)
            _mapGenerator.GenerateMap(graph.Nodes.First());
    }
}
