using SimpleBehaviourTree;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeSettings : ScriptableObject
{
    public const string SettingsPath = "Assets/Settings/BehaviourTreeSettings.asset";

    private static BehaviourTreeSettings _instance;
    internal static BehaviourTreeSettings Instance
    {
        get
        {
            if (_instance == null)
                _instance = GetOrCreateSettings();
            return _instance;
        }
    }

    public static VisualTreeAsset BehaviourTreeUXML => Instance._behaviourTreeUXML;
    public static StyleSheet BehaviourTreeUSS => Instance._behaviourTreeUSS;
    public static VisualTreeAsset NodeUXML => Instance._nodeUXML;
    public static StyleSheet NodeUSS => Instance._nodeUSS;

    [SerializeField] private VisualTreeAsset _behaviourTreeUXML;
    [SerializeField] private StyleSheet _behaviourTreeUSS;
    [SerializeField] private VisualTreeAsset _nodeUXML;
    [SerializeField] private StyleSheet _nodeUSS;

    internal static BehaviourTreeSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(SettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<BehaviourTreeSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
        }
        if(settings._behaviourTreeUXML == null)
        {
            settings._behaviourTreeUXML = 
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(BehaviourTreeEditor))[..^3] + ".uxml", 
                    typeof(VisualTreeAsset)) 
                as VisualTreeAsset;
        }
        if (settings._behaviourTreeUSS == null)
        {
            settings._behaviourTreeUSS =
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(BehaviourTreeEditor))[..^3] + ".uss",
                    typeof(StyleSheet))
                as StyleSheet;
        }
        if (settings._nodeUXML == null)
        {
            settings._nodeUXML = 
                AssetDatabase.LoadAssetAtPath(
                    Utility.GetScriptPath(typeof(NodeView))[..^3] + ".uxml", 
                    typeof(VisualTreeAsset)) 
                as VisualTreeAsset;
        }
        if(settings._nodeUSS == null)
        {
            settings._nodeUSS = 
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
                EditorGUILayout.PropertyField(settings.FindProperty("_behaviourTreeUXML"));
                EditorGUILayout.PropertyField(settings.FindProperty("_behaviourTreeUSS"));
                EditorGUILayout.PropertyField(settings.FindProperty("_nodeUXML"));
                EditorGUILayout.PropertyField(settings.FindProperty("_nodeUSS"));
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
