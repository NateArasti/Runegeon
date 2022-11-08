using UnityEngine;

namespace SimpleBehaviourTree
{
    public class RepeatNode : DecoratorNode
    {
        [SerializeField] private bool m_ExitOnFailure;

        public override string DisplayName => "Repeat";

        protected override State OnUpdate()
        {
            var state = child.Update();
            if(m_ExitOnFailure && state == State.Failure) return State.Failure;
            if(state != State.Running) child.DiscardState();
            return State.Running;
        }
    }
}