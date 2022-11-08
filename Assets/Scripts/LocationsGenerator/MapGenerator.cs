using BehavioursRectangularGraph;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnPivot;
    [SerializeField] private RawImage m_Image;
    [SerializeField] private float m_MapScale = 2f;

    public void GenerateMap(RectangularNode<Room> startNode)
    {

#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            m_SpawnPivot.DestroyChildren();
        else
        {
            var children = new List<GameObject>();
            foreach (Transform child in m_SpawnPivot)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => DestroyImmediate(child));
        }
#else
        _spawnPivot.DestroyChildren();
#endif

        var visitedNodes = new HashSet<RectangularNode<Room>>();

        var image = Instantiate(m_Image, m_SpawnPivot);
        image.texture = startNode.ReferenceBehaviour.MapRender;
        image.SetNativeSize();
        image.rectTransform.sizeDelta *= m_MapScale;

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

                    var image = Instantiate(m_Image, m_SpawnPivot);
                    image.texture = neighbours[i].ReferenceBehaviour.MapRender;
                    image.SetNativeSize();
                    image.rectTransform.sizeDelta *= m_MapScale;

                    var sumSizeDelta = spawnedRender.rectTransform.sizeDelta + image.rectTransform.sizeDelta;

                    var delta = neighbours[i].ReferenceBehaviour.LocalBounds.center + neighbours[i].NodeWorldPosition
                        - (currentNode.ReferenceBehaviour.LocalBounds.center + currentNode.NodeWorldPosition);

                    delta *= m_MapScale * currentNode.ReferenceBehaviour.PixelsPerUnit;

                    image.rectTransform.anchoredPosition = spawnedRender.rectTransform.anchoredPosition + (Vector2)delta;

                    spawnNeighbours(neighbours[i], image);
                }
            }
        }
    }
}
