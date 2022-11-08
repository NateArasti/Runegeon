using SimpleBehaviourTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackboardView : IMGUIContainer
{
    public Blackboard Blackboard { get; set; }

    private string m_NewVariableName = "variable";
    private int m_SelectedTypeIndex = 0;
    private Vector2 m_ScrollPosition;

    public class BlackboardUxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }

    public BlackboardView() { }

    public void HandleGUI()
    {
        EditorGUILayout.BeginHorizontal();
        m_NewVariableName = EditorGUILayout.TextField(m_NewVariableName);
        m_SelectedTypeIndex = EditorGUILayout.Popup(m_SelectedTypeIndex, Blackboard.TypeNames);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Add variable"))
        {
            var type = Blackboard.AvaiableTypes[m_SelectedTypeIndex];
            Blackboard.AddObjectToData(m_NewVariableName, type);
        }
        if (Blackboard != null)
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            Blackboard.DrawGUI();
            EditorGUILayout.EndScrollView();
        }
    }
}