using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : RectangularNodeBehavior
{
    [BoxGroup("Shape"), SerializeField] private Tilemap _shapeReferenceTilemap;
    [BoxGroup("Shape"), SerializeField, ReadOnly] private Bounds _localBounds;

    [BoxGroup("MapRender"), SerializeField, ShowAssetPreview] private Texture2D _mapShapeRender;
    [BoxGroup("MapRender"), SerializeField, Range(1, 16)] private int _pixelsPerUnit = 3;
    [BoxGroup("MapRender"), SerializeField] private Color _shapeBoundsColor = Color.red;
    [BoxGroup("MapRender"), SerializeField] private string _renderSaveFolder;

    [Foldout("Exits"), SerializeField] private Transform[] _topExits;
    [Foldout("Exits"), SerializeField] private Transform[] _rightExits;
    [Foldout("Exits"), SerializeField] private Transform[] _leftExits;
    [Foldout("Exits"), SerializeField] private Transform[] _bottomExits;

    public override IReadOnlyList<Transform> TopExits => _topExits;
    public override IReadOnlyList<Transform> RightExits => _rightExits;
    public override IReadOnlyList<Transform> LeftExits => _leftExits;
    public override IReadOnlyList<Transform> BottomExits => _bottomExits;

    public Texture2D MapRender => _mapShapeRender;

    public Bounds LocalBounds => _localBounds;

    public int PixelsPerUnit => _pixelsPerUnit;

    public void SetRoomColor(Color color) => _shapeReferenceTilemap.color = color;

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
        var worldBounds = _localBounds;
        worldBounds.center += behaviourWorldPosition;
        var otherWorldBounds = otherRoom._localBounds;
        otherWorldBounds.center += otherBehaviorWorldPosition;

        return worldBounds.Intersects(otherWorldBounds);
    }

#if UNITY_EDITOR

    [ContextMenu("GetBounds"), Button("GetBounds")]
    private void GetLocalBounds()
    {
        var temp = Object.Instantiate(this);
        _localBounds = temp._shapeReferenceTilemap.localBounds;
        Object.DestroyImmediate(temp.gameObject);
    }

    [ContextMenu("CreateMapRender"), Button("CreateMapRender")]
    private void CreateMapRender()
    {
        if(_shapeReferenceTilemap == null)
        {
            _shapeReferenceTilemap = GetComponentInChildren<Tilemap>();
        }
        if(_shapeReferenceTilemap == null)
        {
            Debug.LogWarning("No tilemap to render");
            return;
        }
        _shapeReferenceTilemap.CompressBounds();

        // Getting cells render
        var localBound = _localBounds;
        var cellSize = _shapeReferenceTilemap.cellSize;
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
        _mapShapeRender = new Texture2D(renderWidth, renderHeight);
        _mapShapeRender.filterMode = FilterMode.Point;
        _mapShapeRender.SetPixels32(oneDimensionPixels);
        _mapShapeRender.Apply();

        var path = Directory.Exists($"{Application.dataPath}/{_renderSaveFolder}")
            ? $"/{_renderSaveFolder}/{name}_ShapeRender.png"
            : $"/{name}_ShapeRender.png";

        File.WriteAllBytes($"{Application.dataPath}{path}", _mapShapeRender.EncodeToPNG());
        AssetDatabase.ImportAsset($"Assets{path}");
        AssetDatabase.Refresh();
        _mapShapeRender = AssetDatabase.LoadAssetAtPath($"Assets{path}", typeof(Texture2D)) as Texture2D;
        EditorUtility.SetDirty(this);
    }

    private Color32[,] GetCellsRender(int width, int height, Vector3 startPoint)
    {
        var cellSize = _shapeReferenceTilemap.cellSize;
        var cellsRender = new Color32[width, height];
        for (var i = 0; i < width; ++i)
        {
            for (var j = 0; j < height; ++j)
            {
                var point = startPoint + new Vector3(i * cellSize.x, j * cellSize.y);
                var sprite = _shapeReferenceTilemap
                    .GetSprite(_shapeReferenceTilemap.WorldToCell(point));
                cellsRender[i, j] = sprite != null ? _shapeBoundsColor : Color.clear;
            }
        }

        return cellsRender;
    }

    private Color32[,] GetPixelColors(int width, int height, Color32[,] cellsRender, Vector3 startPoint)
    {
        var cellSize = _shapeReferenceTilemap.cellSize;
        var renderWidth = _pixelsPerUnit * width;
        var renderHeight = _pixelsPerUnit * height;
        var pixels = new Color32[renderWidth, renderHeight];
        // TODO: reduce code duplications
        for (var i = 0; i < width; ++i)
        {
            var inner = false;
            for (var j = 0; j <= height; ++j)
            {
                var point = startPoint + new Vector3(i * cellSize.x, j * cellSize.y);
                var cell = _shapeReferenceTilemap.WorldToCell(point);

                if (j == height)
                {
                    if (!inner || CheckIfCellIsExit(cell, RectangularDirection.Up))
                        break;

                    for (var x = i * _pixelsPerUnit; x < (i + 1) * _pixelsPerUnit; ++x)
                    {
                        pixels[x, renderHeight - 1] = _shapeBoundsColor;
                    }
                    break;
                }

                if (!inner && cellsRender[i, j] != Color.clear)
                {
                    inner = true;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Down)) continue;
                    for (var x = i * _pixelsPerUnit; x < (i + 1) * _pixelsPerUnit; ++x)
                    {
                        pixels[x, j * _pixelsPerUnit] = _shapeBoundsColor;
                    }
                }
                else if(inner && cellsRender[i, j] == Color.clear)
                {
                    inner = false;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Up)) continue;
                    for (var x = i * _pixelsPerUnit; x < (i + 1) * _pixelsPerUnit; ++x)
                    {
                        pixels[x, j * _pixelsPerUnit - 1] = _shapeBoundsColor;
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
                var cell = _shapeReferenceTilemap.WorldToCell(point);

                if (i == width)
                {
                    if (!inner || CheckIfCellIsExit(cell, RectangularDirection.Right))
                        break;

                    for (var y = j * _pixelsPerUnit; y < (j + 1) * _pixelsPerUnit; ++y)
                    {
                        pixels[renderWidth - 1, y] = _shapeBoundsColor;
                    }
                    break;
                }

                if (!inner && cellsRender[i, j] != Color.clear)
                {
                    inner = true;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Left)) continue;
                    for (var y = j * _pixelsPerUnit; y < (j + 1) * _pixelsPerUnit; ++y)
                    {
                        pixels[i * _pixelsPerUnit, y] = _shapeBoundsColor;
                    }
                }
                else if(inner && cellsRender[i, j] == Color.clear)
                {
                    inner = false;
                    if (CheckIfCellIsExit(cell, RectangularDirection.Right)) continue;
                    for (var y = j * _pixelsPerUnit; y < (j + 1) * _pixelsPerUnit; ++y)
                    {
                        pixels[i * _pixelsPerUnit - 1, y] = _shapeBoundsColor;
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
            if (cell == _shapeReferenceTilemap.WorldToCell(exit.position)) return true;
        }
        return false;
    }

#endif
}
