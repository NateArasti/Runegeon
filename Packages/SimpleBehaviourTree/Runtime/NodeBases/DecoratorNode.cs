using UnityEngine;

namespace SimpleBehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node child;

        internal override void DiscardState()
        {
            base.DiscardState();
            child.DiscardState();
        }

        internal override Node Clone(GameObject executorObject, Blackboard blackboard)
        {
            var node = base.Clone(executorObject, blackboard) as DecoratorNode;
            node.child = child.Clone(executorObject, blackboard);
            return node;
        }
    }
}
