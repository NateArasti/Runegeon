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

        [SerializeField] private string m_Message;
        [SerializeField] private NodeLogType m_LogType = NodeLogType.Log;

        public override string DisplayName => "Debug Log";

        protected override State OnUpdate()
        {
            switch (m_LogType)
            {
                case NodeLogType.Log:
                    Debug.Log(m_Message);
                    break;
                case NodeLogType.Warning:
                    Debug.LogWarning(m_Message);
                    break;
                case NodeLogType.Error:
                    Debug.LogError(m_Message);
                    break;
            }
            return State.Success;
        }
    }
}