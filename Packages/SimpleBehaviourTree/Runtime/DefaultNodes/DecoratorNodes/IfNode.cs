using UnityEngine;

namespace SimpleBehaviourTree
{
    public class IfNode : DecoratorNode
    {
        [SerializeField] private string[] m_ConditionNames;

        public override string DisplayName => "If";

        protected override State OnUpdate()
        {
            if (CheckAllConditions())
            {
                child.Update();
                return State.Success;
            }
            child.DiscardState();
            return State.Failure;
        }

        private bool CheckAllConditions()
        {
            foreach (var condition in m_ConditionNames)
            {
                if (m_Blackboard.TryGetBool(condition, out var value) && value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}