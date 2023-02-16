using QuickEye.Utility;
using UnityEngine;

public class PixelMap : ScriptableObject
{
    public UnityDictionary<Color32, Vector2Int> lookup = new();
    public Color32[] data;
}