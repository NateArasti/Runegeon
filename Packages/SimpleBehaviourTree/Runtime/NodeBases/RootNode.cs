using UnityEngine;

namespace SimpleBehaviourTree
{
    public sealed class RootNode : Node
    {
        [HideInInspector] public Node child;

        public override string DisplayName => "Root";

        protected override State OnUpdate()
        {
            return child.Update();
        }

        internal override void DiscardState()
        {
            base.DiscardState();
            child.DiscardState();
        }

        internal override Node Clone(GameObject executorObject, Blackboard blackboard)
        {
            var node = base.Clone(executorObject, blackboard) as RootNode;
            node.child = child.Clone(executorObject, blackboard);
            return node;
        }
    }
}