using BehavioursRectangularGraph;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform _spawnPivot;
    [SerializeField] private RawImage _image;
    [SerializeField] private float _mapScale = 2f;

    public void GenerateMap(RectangularNode<Room> startNode)
    {

#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            _spawnPivot.DestroyChildren();
        else
        {
            var children = new List<GameObject>();
            foreach (Transform child in _spawnPivot)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => DestroyImmediate(child));
        }
#else
        _spawnPivot.DestroyChildren();
#endif

        var visitedNodes = new HashSet<RectangularNode<Room>>();

        var image = Instantiate(_image, _spawnPivot);
        image.texture = startNode.ReferenceBehaviour.MapRender;
        image.SetNativeSize();
        image.rectTransform.sizeDelta *= _mapScale;

        spawnNeighbours(startNode, image);

        void spawnNeighbours(RectangularNode<Room> currentNode, RawImage spawnedRender)
        {
            visitedNodes.Add(currentNode);
            foreach (var direction in BehavioursRectangularGraph.Utility.GetEachDirection())
            {
                var neighbours = currentNode.GetNeigboursByDirection(direction);
                for (var i = 0; i < neighbours.Length; ++i)
                {
                    if (visitedNodes.Contains(neighbours[i])) continue;
                    var image = Instantiate(_image, _spawnPivot);
                    image.texture = neighbours[i].ReferenceBehaviour.MapRender;
                    image.SetNativeSize();
                    image.rectTransform.sizeDelta *= _mapScale;

                    var sumSizeDelta = spawnedRender.rectTransform.sizeDelta + image.rectTransform.sizeDelta;

                    var delta = neighbours[i].ReferenceBehaviour.LocalBounds.center + neighbours[i].NodeWorldPosition
                        - (currentNode.ReferenceBehaviour.LocalBounds.center + currentNode.NodeWorldPosition);

                    delta *= _mapScale * currentNode.ReferenceBehaviour.PixelsPerUnit;

                    image.rectTransform.anchoredPosition = spawnedRender.rectTransform.anchoredPosition + (Vector2)delta;

                    spawnNeighbours(neighbours[i], image);
                }
            }
        }
    }
}
