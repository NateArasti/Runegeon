using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationGenerator : MonoBehaviour
{
    [SerializeField] private Room[] _roomsPrefabs;

    [BoxGroup("Random"), SerializeField] private bool _customSeed;
    [BoxGroup("Random"), SerializeField, ShowIf(nameof(_customSeed))] private int _seed;

    [BoxGroup("Generation Params"), SerializeField, MinValue(2), MaxValue(10)] private int _maxDepth = 5;
    [BoxGroup("Generation Params"), SerializeField, Range(0, 1)] private float _deadEndChance = 0.1f;
    [BoxGroup("Generation Params"), SerializeField] private bool _handleCycles = true;
    [BoxGroup("Generation Params"), SerializeField] private Gradient _depthGradient;

    private void Awake()
    {
        if (!_customSeed)
        {
            _seed = System.BitConverter.ToInt32(System.Guid.NewGuid().ToByteArray());
        }
        Random.InitState(_seed);
    }

    [Button("Check Weight Shuffle")]
    private void CheckWeightShuffle()
    {
        _roomsPrefabs = Utility.GetWeightedShuffle(_roomsPrefabs.ToList()).ToArray();
    }

    [Button("Generate")]
    public void Generate()
    {
#if UNITY_EDITOR
        Awake();

        var children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => DestroyImmediate(child));
#else
        transform.DestroyChildren();
#endif

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

                //for demo puprose
                room.SetRoomColor(_depthGradient.Evaluate((float)node.Depth / _maxDepth));
            }

            //var startNode = graph.Nodes.First();
            //var connectedNodes = new HashSet<RectangularNode<Room>>();
            //CountAllConnectedNodes(startNode, ref connectedNodes);
            //Debug.LogWarning($"{graph.Nodes.Count} - {connectedNodes.Count}");
        }
        var result = success ? "success" : "fail";
        Debug.Log($"Generation finished with result - {result}");
    }

    private void CountAllConnectedNodes(RectangularNode<Room> node, ref HashSet<RectangularNode<Room>> connectedNodes)
    {
        foreach(var neighbour in node.LeftNeighbours)
        {
            if(connectedNodes.Contains(neighbour)) continue;
            connectedNodes.Add(neighbour);
            CountAllConnectedNodes(neighbour, ref connectedNodes);
        }
        foreach(var neighbour in node.RightNeighbours)
        {
            if (connectedNodes.Contains(neighbour)) continue;
            connectedNodes.Add(neighbour);
            CountAllConnectedNodes(neighbour, ref connectedNodes);
        }
        foreach(var neighbour in node.TopNeighbours)
        {
            if (connectedNodes.Contains(neighbour)) continue;
            connectedNodes.Add(neighbour);
            CountAllConnectedNodes(neighbour, ref connectedNodes);
        }
        foreach(var neighbour in node.BottomNeighbours)
        {
            if (connectedNodes.Contains(neighbour)) continue;
            connectedNodes.Add(neighbour);
            CountAllConnectedNodes(neighbour, ref connectedNodes);
        }
    }
}
