using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : RectangularNodeBehavior
{
    [SerializeField] private Transform m_StartPosition;

    [BoxGroup("Shape"), SerializeField] private Tilemap m_ShapeReferenceTilemap;
    [BoxGroup("Shape"), SerializeField, ReadOnly] private Bounds m_LocalBounds;

    [BoxGroup("MapRender"), SerializeField, ShowAssetPreview] private Texture2D m_MapShapeRender;
    [BoxGroup("MapRender"), SerializeField, Range(1, 16)] private int m_PixelsPerUnit = 3;
    [BoxGroup("MapRender"), SerializeField] private Color m_ShapeBoundsColor = Color.red;
    [BoxGroup("MapRender"), SerializeField] private string m_RenderSaveFolder;

    [Foldout("Exits"), SerializeField] private Transform[] m_TopExits;
    [Foldout("Exits"), SerializeField] private Transform[] m_RightExits;
    [Foldout("Exits"), SerializeField] private Transform[] m_LeftExits;
    [Foldout("Exits"), SerializeField] private Transform[] m_BottomExits;

    public int Depth { get; set; }

    public override IReadOnlyList<Transform> TopExits => m_TopExits;
    public override IReadOnlyList<Transform> RightExits => m_RightExits;
    public override IReadOnlyList<Transform> LeftExits => m_LeftExits;
    public override IReadOnlyList<Transform> BottomExits => m_BottomExits;

    public Vector2 StartPosition => m_StartPosition.position;

    public Texture2D MapRender => m_MapShapeRender;
    public Bounds LocalBounds => m_LocalBounds;
    public int PixelsPerUnit => m_PixelsPerUnit;

    public void SetRoomColor(Color color) => m_ShapeReferenceTilemap.color = color;

    public override bool IsCompatable(
        Vector3 behaviourWorldPosition, 
        RectangularNodeBehavior otherBehavior, 
        Vector3 otherBehaviorWorldPosition)
    {
        if(otherBehavior is Room otherRoom)
        {
            if(Intersects(behaviourWorldPosition, otherRoom, otherBehaviorWorldPosition))
            {
                return false;
            }

            //TODO: Implement normal checking of too close but not intersecting placement
            // It would prevent some bad scenarios on earlier stage

            return true;
        }
        return false;
    }

    private bool Intersects(
        Vector3 behaviourWorldPosition,
        Room otherRoom,
        Vector3 otherBehaviorWorldPosition)
    {
        var worldBounds = m_LocalBounds;
        worldBounds.center += behaviourWorldPosition;
        var otherWorldBounds = otherRoom.m_LocalBounds;
        otherWorldBounds.center += otherBehaviorWorldPosition;

        return worldBounds.Intersects(otherWorldBounds);
    }

#if UNITY_EDITOR

    [ContextMenu("GetBounds"), Button("GetBounds")]
    private void GetLocalBounds()
    {
        var temp = Object.Instantiate(this);
        m_LocalBounds = temp.m_ShapeReferenceTilemap.localBounds;
        Object.DestroyImmediate(temp.gameObject);
    }

    [ContextMenu("CreateMapRender"), Button("CreateMapRender")]
    private void CreateMapRender()
    {
        if(m_ShapeReferenceTilemap == null)
        {
            m_ShapeReferenceTilemap = GetComponentInChildren<Tilemap>();
        }
        if(m_ShapeReferenceTilemap == null)
        {
            Debug.LogWarning("No tilemap to render");
            return;
        }
        m_ShapeReferenceTilemap.CompressBounds();

        // Getting cells render
        var localBound = m_LocalBounds;
        var cellSize = m_ShapeReferenceTilemap.cellSize;
        var width = (int)(localBound.size.x / cellSize.x);
        var height = (int)(localBound.size.y / cellSize.y);
        var cellsRender = GetCellsRender(width, height, localBound.min);

        // Setting pixels
        var pixels = GetPixelColors(width, height, cellsRender, localBound.min);
        var renderWidth = pixels.GetLength(0);
        var renderHeight = pixels.GetLength(1);

        // Saving render
        var oneDimensionPixels = new Color32[renderWidth * renderHeight];
        var index = 0;
        for (var j = 0; j < renderHeight; ++j)
        {
            for (var i = 0; i < renderWidth; ++i)
            {
                oneDimensionPixels[index] = pixels[i, j];
                index++;
            }
        }
        m_MapShapeRender = new Texture2D(renderWidth, renderHeight);
        m_MapShapeRender.filterMode = FilterMode.Point;
        m_MapShapeRender.SetPixels32(oneDimensionPixels);
        m_MapShapeRender.Apply();

        var path = Directory.Exists($"{Application.dataPath}/{m_RenderSaveFolder}")
            ? $"/{m_RenderSaveFolder}/{name}_ShapeRender.png"
            : $"/{name}_ShapeRender.png";

        File.WriteAllBytes($"{Application.dataPath}{path}", m_MapShapeRender.EncodeToPNG());
        AssetDatabase.ImportAsset($"Assets{path}");
        AssetDatabase.Refresh();
        m_MapShapeRender = AssetDatabase.LoadAssetAtPath($"Assets{path}", typeof(Texture2D)) as Texture2D;
        EditorUtility.SetDirty(this);
    }

    private Color32[,] GetCellsRender(int width, int height, Vector3 startPoint)
    {
        var cellSize = m_ShapeReferenceTilemap.cellSize;
        var cellsRender = new Color32[width, height];
        for (var i = 0; i < width; ++i)
        {
            for (var j = 0; j < height; ++j)
            {
                var point = startPoint + new Vector3(i * cellSize.x, j * cellSize.y);
                var sprite = m_ShapeReferenceTilemap
                    .GetSprite(m_ShapeReferenceTilemap.WorldToCell(point));
                cellsRender[i, j] = sprite != null ? m_ShapeBoundsColor : Color.clear;
            }
        }

        return cellsRender;
    }

    private Color32[,] GetPixelColors(int width, int height, Color32[,] cellsRender, Vector3 startPoint)
    {
        var cellSize = m_ShapeReferenceTilemap.cellSize;
        var renderWidth = m_PixelsPerUnit * width;
        var renderHeight = m_PixelsPerUnit * height;
        var pixels = new Color32[renderWidth, renderHeight];
        // TODO: reduce code duplications
        for (var i = 0; i < width; ++i)
        {
            var inner = false;
            for (var j = 0; j <= height; ++j)
            {
                var point = startPoint + new Vector3(i * cellSize.x, j * cellSize.y);
                var cell = m_ShapeReferenceTilemap.WorldToCell(point);

                if (j == height)
                {
                    if (!inner || CheckIfCellIsExit(cell, RectangularDirection.Up))
                        break;

                    for (var x = i * m_PixelsPerUnit; x < (i + 1) * m_PixelsPerUnit; ++x)
                    {
                        pixels[x, renderHeight - 1] = m_ShapeBoundsColor;
                    }
                    break;
                }

                if (!inner && cellsRender[i, j] != Color.clear)
                {
                    inner = true;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Down)) continue;
                    for (var x = i * m_PixelsPerUnit; x < (i + 1) * m_PixelsPerUnit; ++x)
                    {
                        pixels[x, j * m_PixelsPerUnit] = m_ShapeBoundsColor;
                    }
                }
                else if(inner && cellsRender[i, j] == Color.clear)
                {
                    inner = false;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Up)) continue;
                    for (var x = i * m_PixelsPerUnit; x < (i + 1) * m_PixelsPerUnit; ++x)
                    {
                        pixels[x, j * m_PixelsPerUnit - 1] = m_ShapeBoundsColor;
                    }
                }
            }
        }
        for (var j = 0; j < height; ++j)
        {
            var inner = false;
            for (var i = 0; i <= width; ++i)
            {
                var point = startPoint + new Vector3(i * cellSize.x, j * cellSize.y);
                var cell = m_ShapeReferenceTilemap.WorldToCell(point);

                if (i == width)
                {
                    if (!inner || CheckIfCellIsExit(cell, RectangularDirection.Right))
                        break;

                    for (var y = j * m_PixelsPerUnit; y < (j + 1) * m_PixelsPerUnit; ++y)
                    {
                        pixels[renderWidth - 1, y] = m_ShapeBoundsColor;
                    }
                    break;
                }

                if (!inner && cellsRender[i, j] != Color.clear)
                {
                    inner = true;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Left)) continue;
                    for (var y = j * m_PixelsPerUnit; y < (j + 1) * m_PixelsPerUnit; ++y)
                    {
                        pixels[i * m_PixelsPerUnit, y] = m_ShapeBoundsColor;
                    }
                }
                else if(inner && cellsRender[i, j] == Color.clear)
                {
                    inner = false;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Right)) continue;
                    for (var y = j * m_PixelsPerUnit; y < (j + 1) * m_PixelsPerUnit; ++y)
                    {
                        pixels[i * m_PixelsPerUnit - 1, y] = m_ShapeBoundsColor;
                    }
                }
            }
        }
        return pixels;
    }

    private bool CheckIfCellIsExit(Vector3Int cell, RectangularDirection direction)
    {
        var exits = GetExitsByDirection(direction);
        foreach (var exit in exits)
        {
            if (cell == m_ShapeReferenceTilemap.WorldToCell(exit.position)) return true;
        }
        return false;
    }

#endif
}
