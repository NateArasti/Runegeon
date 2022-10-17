using UnityEngine;

namespace SimpleBehaviourTree
{
    public class RepeatNode : DecoratorNode
    {
        [SerializeField] private bool _exitOnFailure;

        public override string DisplayName => "Repeat";

        protected override State OnUpdate()
        {
            var state = child.Update();
            if(_exitOnFailure && state == State.Failure) return State.Failure;
            return State.Running;
        }
    }
}