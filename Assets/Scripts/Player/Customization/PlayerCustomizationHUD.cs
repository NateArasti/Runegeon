using QuickEye.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

public class PlayerCustomizationHUD : MonoBehaviour
{
    private const string k_CustomizationSeen = "CustomizationSeen";
    private const string k_CustomizationLayerPrefix = "CustomizationLayer_";

    [SerializeField] private CustomizationData m_CustomizationData;
    [SerializeField] private UnityDictionary<string, Texture2D[]> m_LayerVariations;
    private readonly Dictionary<string, int> m_CurrentLayerIndexes = new();

    private void Start()
    {
        var customizationSeen = PlayerPrefs.GetInt(k_CustomizationSeen, 0) == 1;
        if (customizationSeen)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        LoadCustomizationData();
    }

    private void OnDisable()
    {
        SaveCustomizationData();
    }

    private void LoadCustomizationData()
    {
        foreach (var layer in m_LayerVariations)
        {
            var index = PlayerPrefs.GetInt($"{k_CustomizationLayerPrefix}{layer.Key}", 0);
            m_CurrentLayerIndexes[layer.Key] = MathExtensions.ClampListIndex(index, layer.Value.Length);
            m_CustomizationData.SetLayerData(layer.Key, layer.Value[m_CurrentLayerIndexes[layer.Key]]);
        }
    }

    public void SaveCustomizationData()
    {
        PlayerPrefs.SetInt(k_CustomizationSeen, 1);
        foreach (var layer in m_LayerVariations)
        {
            PlayerPrefs.SetInt($"{k_CustomizationLayerPrefix}{layer.Key}", m_CurrentLayerIndexes[layer.Key]);
            
        }
        PlayerPrefs.Save();
    }

    public void DecreaseLayerSkinIndex(string layer)
    {
        SetLayerIndex(layer, m_CurrentLayerIndexes[layer] - 1);
    }

    public void IncreaseLayerSkinIndex(string layer)
    {
        SetLayerIndex(layer, m_CurrentLayerIndexes[layer] + 1);
    }

    private void SetLayerIndex(string layer, int index)
    {
        index = MathExtensions.ClampListIndex(index, m_LayerVariations[layer].Length);
        m_CurrentLayerIndexes[layer] = index;
        m_CustomizationData.SetLayerData(layer, m_LayerVariations[layer][index]);
    }
}
