using SimpleBehaviourTree;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public event Action<NodeView> OnNodeSelected;

    public UnityEditor.Experimental.GraphView.Port input;
    public UnityEditor.Experimental.GraphView.Port output;

    public int InputIndex = -1;

    public Node Node { get; private set; }

    public NodeView(Node node) : base(AssetDatabase.GetAssetPath(BehaviourTreeSettings.NodeUXML))
    {
        styleSheets.Add(BehaviourTreeSettings.NodeUSS);
        Node = node;
        title = Node.DisplayName;
        viewDataKey = Node.nodeGUID;

        style.left = Node.GraphPosition.x;
        style.top = Node.GraphPosition.y;

        CreateInputPorts();
        CreateOutputPorts();
        AddUSSClasses();
    }

    private void AddUSSClasses()
    {
        const string rootClass = "root";
        const string actionClass = "action";
        const string compositionClass = "composition";
        const string decoratorClass = "decorator";

        if (Node is RootNode)
        {
            AddToClassList(rootClass);
        }
        else if(Node is ActionNode)
        {
            AddToClassList(actionClass);
        }
        else if(Node is CompositionNode)
        {
            AddToClassList(compositionClass);
        }
        else if(Node is DecoratorNode)
        {
            AddToClassList(decoratorClass);
        }
    }

    private void CreateInputPorts()
    {
        if(Node is ActionNode or CompositionNode or DecoratorNode)
        {
            input = InstantiatePort(
                UnityEditor.Experimental.GraphView.Orientation.Vertical,
                UnityEditor.Experimental.GraphView.Direction.Input,
                UnityEditor.Experimental.GraphView.Port.Capacity.Single,
                typeof(bool));
        }

        if(input != null)
        {
            SetInputPort();
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    public void SetInputPort()
    {
        input.portName = InputIndex == -1 ? "" : InputIndex.ToString();
    }

    private void CreateOutputPorts()
    {
        if (Node is RootNode or DecoratorNode)
        {
            output = InstantiatePort(
                UnityEditor.Experimental.GraphView.Orientation.Vertical,
                UnityEditor.Experimental.GraphView.Direction.Output,
                UnityEditor.Experimental.GraphView.Port.Capacity.Single,
                typeof(bool));
        }
        else if (Node is CompositionNode)
        {
            output = InstantiatePort(
                UnityEditor.Experimental.GraphView.Orientation.Vertical,
                UnityEditor.Experimental.GraphView.Direction.Output,
                UnityEditor.Experimental.GraphView.Port.Capacity.Multi,
                typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(Node, $"Behavior Tree ({nameof(SetPosition)})");
        Node.GraphPosition.x = newPos.xMin;
        Node.GraphPosition.y = newPos.yMin;
        EditorUtility.SetDirty(Node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        if(Node is CompositionNode compositionNode)
        {
            compositionNode.children.Sort((left, right) => 
                left.GraphPosition.x.CompareTo(right.GraphPosition.x)
            );
        }
    }

    public void UpdateState()
    {
        const string runningClass = "running";
        const string successClass = "success";
        const string failureClass = "failure";

        RemoveFromClassList(runningClass);
        RemoveFromClassList(successClass);
        RemoveFromClassList(failureClass);

        if (!Application.isPlaying) return;

        switch (Node.NodeState)
        {
            case Node.State.Running:
                if (Node.Started)
                    AddToClassList(runningClass);
                break;
            case Node.State.Success:
                AddToClassList(successClass);
                break;
            case Node.State.Failure:
                AddToClassList(failureClass);
                break;
        }
    }
}
