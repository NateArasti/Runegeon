using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviourTree
{
    public abstract class CompositionNode : Node
    {
        [HideInInspector] public List<Node> children = new();

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.children = children.ConvertAll(child => child.Clone());
            return node;
        }
    }
}
