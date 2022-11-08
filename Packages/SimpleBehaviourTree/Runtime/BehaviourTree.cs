using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimpleBehaviourTree
{
    [CreateAssetMenu(fileName = "SimpleTree")]
    public class BehaviourTree : ScriptableObject
    {
        public Blackboard blackboard;
        [HideInInspector] public List<Node> nodes = new();
        [HideInInspector] public Node rootNode;

        private GameObject m_ExecutorObject;

        public Node.State TreeState { get; private set; } = Node.State.Running;

        public BehaviourTree Clone(GameObject executorObject)
        {
            var tree = Instantiate(this);
            tree.rootNode = rootNode.Clone(executorObject, tree.blackboard);
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, node =>
            {
                tree.nodes.Add(node);
            });

            tree.m_ExecutorObject = executorObject;

            return tree;
        }

        private void Traverse(Node node, Action<Node> visitNode)
        {
            if(node != null)
            {
                visitNode(node);
                var children = GetChildren(node);
                if (children != null)
                {
                    children.ForEach(child => Traverse(child, visitNode));
                }
            }
        }

        public Node.State Update()
        {
            if(rootNode == null)
            {
                Debug.LogError($"Root Node of {name} is null");
                return Node.State.Failure;
            }
            if(rootNode.NodeState != Node.State.Success || rootNode.NodeState != Node.State.Failure)
            {
                TreeState = rootNode.Update();
            }
            return TreeState;
        }

        public Node CreateNode(Type nodeType)
        {
            var node = ScriptableObject.CreateInstance(nodeType) as Node;
            if (node == null)
            {
                throw new System.NullReferenceException($"Node type wasn't \"Node\" type - {nodeType.Name}");
            }
            node.name = nodeType.Name;
            node.nodeGUID = GUID.Generate().ToString();

            Undo.RecordObject(this, $"BehaviourTree ({nameof(CreateNode)})");

            nodes.Add(node);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);

                Undo.RegisterCreatedObjectUndo(node, $"BehaviourTree ({nameof(CreateNode)})");

                AssetDatabase.SaveAssets();
            }
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, $"BehaviourTree ({nameof(DeleteNode)})");
            nodes.Remove(node);
            if (!Application.isPlaying)
            {
                Undo.DestroyObjectImmediate(node);
                AssetDatabase.SaveAssets();
            }
        }

        public void AddChild(Node parent, Node child)
        {
            if(parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, $"BehaviourTree ({nameof(AddChild)})");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }
            else if(parent is DecoratorNode decoratorNode)
            {
                decoratorNode.child = child;
            }
            else if(parent is CompositionNode compositionNode)
            {
                Undo.RecordObject(compositionNode, $"BehaviourTree ({nameof(AddChild)})");
                compositionNode.children.Add(child);
                EditorUtility.SetDirty(compositionNode);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            if(parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, $"BehaviourTree ({nameof(RemoveChild)})");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }
            else if(parent is DecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, $"BehaviourTree ({nameof(RemoveChild)})");
                decoratorNode.child = null;
                EditorUtility.SetDirty(decoratorNode);
            }
            else if (parent is CompositionNode compositionNode)
            {
                Undo.RecordObject(compositionNode, $"BehaviourTree ({nameof(RemoveChild)})");
                compositionNode.children.Remove(child);
                EditorUtility.SetDirty(compositionNode);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            if (parent is RootNode rootNode)
            {
                if(rootNode.child != null)
                    return new List<Node>() { rootNode.child };
                return null;
            }
            else if (parent is DecoratorNode decoratorNode)
            {
                if(decoratorNode.child != null)
                    return new List<Node>() { decoratorNode.child };
                return null;
            }
            else if (parent is CompositionNode compositionNode)
            {
                return compositionNode.children;
            }
            return null;
        }
    }
}
