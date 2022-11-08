using UnityEngine;

namespace SimpleBehaviourTree
{
    public class SequenceNode : CompositionNode
    {
        [SerializeField] private bool m_WaitForSuccess;
        private int m_CurrentChild;

        public override string DisplayName => "Sequence";

        protected override void OnStart()
        {
            m_CurrentChild = 0;
        }

        protected override void OnStop()
        {
            m_CurrentChild = 0;
        }

        protected override State OnUpdate()
        {
            if (m_CurrentChild >= children.Count)
            {
                Debug.LogWarning($"{name} got out of the children nodes bounds");
                return State.Failure;
            }
            var state = children[m_CurrentChild].Update();
            if (m_WaitForSuccess && state != State.Success)
            {
                return state;
            }
            m_CurrentChild++;
            return m_CurrentChild == children.Count ? State.Success : State.Running;
        }
    }
}