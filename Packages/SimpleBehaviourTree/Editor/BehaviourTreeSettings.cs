using SimpleBehaviourTree;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeSettings : ScriptableObject
{
    public const string SettingsPath = "Assets/Settings/BehaviourTreeSettings.asset";

    private static BehaviourTreeSettings s_Instance;
    internal static BehaviourTreeSettings Instance
    {
        get
        {
            if (s_Instance == null)
                s_Instance = GetOrCreateSettings();
            return s_Instance;
        }
    }

    public static VisualTreeAsset BehaviourTreeUXML => Instance.m_BehaviourTreeUXML;
    public static StyleSheet BehaviourTreeUSS => Instance.m_BehaviourTreeUSS;
    public static VisualTreeAsset NodeUXML => Instance.m_NodeUXML;
    public static StyleSheet NodeUSS => Instance.m_NodeUSS;

    [SerializeField] private VisualTreeAsset m_BehaviourTreeUXML;
    [SerializeField] private StyleSheet m_BehaviourTreeUSS;
    [SerializeField] private VisualTreeAsset m_NodeUXML;
    [SerializeField] private StyleSheet m_NodeUSS;

    internal static BehaviourTreeSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(SettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<BehaviourTreeSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
        }
        if(settings.m_BehaviourTreeUXML == null)
        {
            settings.m_BehaviourTreeUXML = 
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(BehaviourTreeEditor))[..^3] + ".uxml", 
                    typeof(VisualTreeAsset)) 
                as VisualTreeAsset;
        }
        if (settings.m_BehaviourTreeUSS == null)
        {
            settings.m_BehaviourTreeUSS =
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(BehaviourTreeEditor))[..^3] + ".uss",
                    typeof(StyleSheet))
                as StyleSheet;
        }
        if (settings.m_NodeUXML == null)
        {
            settings.m_NodeUXML = 
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(NodeView))[..^3] + ".uxml", 
                    typeof(VisualTreeAsset)) 
                as VisualTreeAsset;
        }
        if(settings.m_NodeUSS == null)
        {
            settings.m_NodeUSS = 
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(NodeView))[..^3] + ".uss", 
                    typeof(StyleSheet)) 
                as StyleSheet;
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}

//Register a SettingsProvider using IMGUI for the drawing framework:
public static class BehaviourTreeSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/BehaviourTreeIMGUISettings", SettingsScope.Project)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "Behaviour Tree Settings",
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = (searchContext) =>
            {
                var settings = BehaviourTreeSettings.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("m_BehaviourTreeUXML"));
                EditorGUILayout.PropertyField(settings.FindProperty("m_BehaviourTreeUSS"));
                EditorGUILayout.PropertyField(settings.FindProperty("m_NodeUXML"));
                EditorGUILayout.PropertyField(settings.FindProperty("m_NodeUSS"));
                settings.ApplyModifiedPropertiesWithoutUndo();
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { 
                "BehaviourTreeUXML", 
                "BehaviourTreeUSS",
                "NodeUXML",
                "NodeUSS",  
            })
        };

        return provider;
    }
}
