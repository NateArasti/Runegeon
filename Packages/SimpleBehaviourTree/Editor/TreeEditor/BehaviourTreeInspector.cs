using System;
using UnityEditor;
using UnityEngine.UIElements;

public class BehaviourTreeInspector : VisualElement
{
    public class BehaviorTreeUxmlFactory : UxmlFactory<BehaviourTreeInspector, VisualElement.UxmlTraits> { }

    public BehaviourTreeInspector() { }

    private Editor editor;

    internal void UpdateNodeSelection(NodeView nodeView)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.Node);
        var container = new IMGUIContainer(() =>
        {
            if(editor.target)
                editor.OnInspectorGUI();
        });
        Add(container);
    }
}
