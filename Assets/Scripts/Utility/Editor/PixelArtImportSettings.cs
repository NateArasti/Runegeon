using System.IO;
using UnityEditor;
using UnityEngine;

public class PixelArtImportSettings : AssetPostprocessor
{
    private const string ExtensionPNG = ".png";

    private void OnPostprocessTexture(Texture2D texture)
    {
        if (!assetPath.Contains("Asset")) return;
        var extension = Path.GetExtension(assetPath);

        if (extension != ExtensionPNG)
            return;

        var textureImporter = assetImporter as TextureImporter;

        if (textureImporter)
        {
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
