using QuickEye.Utility;
using UnityEngine;

[CreateAssetMenu]
public class CustomizationData : ScriptableObject
{
    [SerializeField] private string m_SkinTextureProperty = "_SkinTexture";
    [SerializeField] private UnityDictionary<string, Material> m_CustomizationLayers;

    public void SetLayerData(string layerName, Texture2D skinTexture)
    {
        if (!m_CustomizationLayers.ContainsKey(layerName)) return;
        m_CustomizationLayers[layerName].SetTexture(m_SkinTextureProperty, skinTexture);
    }
}
