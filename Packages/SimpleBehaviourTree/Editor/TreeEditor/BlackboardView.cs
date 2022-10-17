using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackboardView : IMGUIContainer
{
    private Vector2 _scrollPosition;

    public class BlackboardUxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }

    public BlackboardView() { }

    public void HandleGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.LabelField("Implement me, pls");
        EditorGUILayout.EndScrollView();
    }
}