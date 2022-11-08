using System;
using UnityEditor;
using UnityEngine.UIElements;

public class BehaviourTreeInspector : VisualElement
{
    public class BehaviorTreeUxmlFactory : UxmlFactory<BehaviourTreeInspector, VisualElement.UxmlTraits> { }

    public BehaviourTreeInspector() { }

    private Editor m_Editor;

    internal void UpdateNodeSelection(NodeView nodeView)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(m_Editor);
        m_Editor = Editor.CreateEditor(nodeView.Node);
        var container = new IMGUIContainer(() =>
        {
            if(m_Editor.target)
                m_Editor.OnInspectorGUI();
        });
        Add(container);
    }
}
