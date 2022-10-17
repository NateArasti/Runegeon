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

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}