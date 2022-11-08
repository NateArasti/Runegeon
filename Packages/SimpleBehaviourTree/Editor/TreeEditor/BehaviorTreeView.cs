using SimpleBehaviourTree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView : GraphView
{
    public event Action<NodeView> OnNodeSelected;

    private BehaviourTree m_Tree;

    public class BehaviorTreeUxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = BehaviourTreeSettings.BehaviourTreeUSS;
        styleSheets.Add(styleSheet);
        visible = false;

        Undo.undoRedoPerformed += RefreshView;
    }

    private void RefreshView()
    {
        PopulateView(m_Tree);
        AssetDatabase.SaveAssets();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports
            .Where(endPort => 
                endPort.direction != startPort.direction && 
                endPort.node != startPort.node)
            .ToList();
    }

    public void PopulateView(BehaviourTree tree)
    {
        if (tree == null) return;
        visible = true;
        m_Tree = tree;

        graphViewChanged -= OnGraphViewChanged;

        DeleteElements(graphElements);

        graphViewChanged += OnGraphViewChanged;

        if(m_Tree.rootNode == null)
        {
            m_Tree.rootNode = m_Tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(m_Tree);
            AssetDatabase.SaveAssets();
        }

        //Creating nodes views
        foreach(var node in m_Tree.nodes)
        {
            CreateNodeView(node);
        }

        //Creating edges
        foreach(var node in m_Tree.nodes)
        {
            var children = m_Tree.GetChildren(node);
            if (children == null) continue;
            var parentView = GetNodeView(node);
            children.ForEach(child =>
            {
                var childView = GetNodeView(child);

                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        }

        HandleNodeSort();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(element =>
            {
                if (element is NodeView nodeView)
                {
                    m_Tree.DeleteNode(nodeView.Node);
                }
                else if (element is Edge edge)
                {
                    var parent = edge.output.node as NodeView;
                    var child = edge.input.node as NodeView;
                    child.inputIndex = -1;
                    child.SetInputPort();

                    m_Tree.RemoveChild(parent.Node, child.Node);

                    RebuildInputIndexesForChildren(parent);
                }
            });
        }

        if(graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                var parent = edge.output.node as NodeView;
                var child = edge.input.node as NodeView;

                m_Tree.AddChild(parent.Node, child.Node);
                RebuildInputIndexesForChildren(parent);
            });
        }

        if(graphViewChange.movedElements != null)
        {
            HandleNodeSort();
        }

        return graphViewChange;
    }

    private void HandleNodeSort()
    {
        nodes.ForEach((node) =>
        {
            if (node is NodeView nodeView)
            {
                RebuildInputIndexesForChildren(nodeView);
            }
        });
    }

    private void RebuildInputIndexesForChildren(NodeView parentNodeView)
    {
        parentNodeView.SortChildren();
        if (parentNodeView.Node is CompositionNode compositionNode)
        {
            for (var i = 0; i < compositionNode.children.Count; ++i)
            {
                var child = compositionNode.children[i];
                if (GetNodeByGuid(child.nodeGUID) is NodeView childView)
                {
                    childView.inputIndex = i + 1;
                    childView.SetInputPort();
                }
            }
        }
    }

    private NodeView CreateNodeView(SimpleBehaviourTree.Node node)
    {
        var nodeView = new NodeView(node);
        if(node is RootNode)
        {
            nodeView.capabilities ^= Capabilities.Deletable;
        }
        nodeView.OnNodeSelected += OnNodeSelected.Invoke;
        AddElement(nodeView);

        return nodeView;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        var nodeTypes = TypeCache.GetTypesDerivedFrom<SimpleBehaviourTree.Node>();
        var spawnPosition = evt.mousePosition;
        foreach(var nodeType in nodeTypes)
        {
            var types = TypeCache.GetTypesDerivedFrom(nodeType);
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{nodeType.Name}]/{type.Name}", arg =>
                {
                    var nodeView = CreateNode(type);
                    nodeView.SetPosition(new Rect(spawnPosition, Vector2.zero));
                });
            }
        }
    }

    private NodeView GetNodeView(SimpleBehaviourTree.Node node) =>
        GetNodeByGuid(node.nodeGUID) as NodeView;

    private NodeView CreateNode(Type type)
    {
        var node = m_Tree.CreateNode(type);
        return CreateNodeView(node);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(node =>
        {
            if (node is NodeView nodeView)
            {
                nodeView.UpdateState();
            }
        });
    }
}