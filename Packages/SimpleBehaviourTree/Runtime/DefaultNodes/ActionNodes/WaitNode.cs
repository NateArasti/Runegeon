using UnityEngine;

namespace SimpleBehaviourTree
{
    public class WaitNode : ActionNode
    {
        [SerializeField] private float _waitDuration;
        [SerializeField] private bool _waitUnscaled;

        private float _currentTime;

        public override string DisplayName => "Wait";

        protected override void OnStart()
        {
            _currentTime = _waitDuration;
        }

        protected override State OnUpdate()
        {
            _currentTime -= _waitUnscaled ? Time.unscaledTime : Time.deltaTime;
            if(_currentTime <= 0)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}