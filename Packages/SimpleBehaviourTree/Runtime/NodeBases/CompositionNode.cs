using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviourTree
{
    public abstract class CompositionNode : Node
    {
        [HideInInspector] public List<Node> children = new();

        internal override void DiscardState()
        {
            base.DiscardState();
            foreach (var child in children)
            {
                child.DiscardState();
            }
        }

        internal override Node Clone(GameObject executorObject, Blackboard blackboard)
        {
            var node = base.Clone(executorObject, blackboard) as CompositionNode;
            node.children = children.ConvertAll(child => child.Clone(executorObject, blackboard));
            return node;
        }
    }
}
