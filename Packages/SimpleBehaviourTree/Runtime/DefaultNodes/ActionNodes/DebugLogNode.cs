using UnityEngine;

namespace SimpleBehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        public enum NodeLogType
        {
            Log,
            Warning,
            Error
        }

        [SerializeField] private string _message;
        [SerializeField] private NodeLogType _logType = NodeLogType.Log;

        public override string DisplayName => "Debug Log";

        protected override State OnUpdate()
        {
            switch (_logType)
            {
                case NodeLogType.Log:
                    Debug.Log(_message);
                    break;
                case NodeLogType.Warning:
                    Debug.LogWarning(_message);
                    break;
                case NodeLogType.Error:
                    Debug.LogError(_message);
                    break;
            }
            return State.Success;
        }
    }
}