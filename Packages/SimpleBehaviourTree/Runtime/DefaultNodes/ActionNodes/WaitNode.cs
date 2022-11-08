using UnityEngine;

namespace SimpleBehaviourTree
{
    public class WaitNode : ActionNode
    {
        [SerializeField] private float m_WaitDuration;
        [SerializeField] private bool m_WaitUnscaled;

        private float m_CurrentTime;

        public override string DisplayName => "Wait";

        protected override void OnStart()
        {
            m_CurrentTime = m_WaitDuration;
        }

        protected override State OnUpdate()
        {
            m_CurrentTime -= m_WaitUnscaled ? Time.unscaledTime : Time.deltaTime;
            if(m_CurrentTime <= 0)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}