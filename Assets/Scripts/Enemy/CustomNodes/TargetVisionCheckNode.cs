using UnityEngine;
using SimpleBehaviourTree;

public class TargetVisionCheckNode : ActionNode
{
    [SerializeField] private string m_BlackboardProperty;
    private IVisionChecker m_VisionChecker;

    public override string DisplayName => "Check Sight";

    protected override void OnStart()
    {
        m_VisionChecker = m_ExecutorObject.GetComponent<IVisionChecker>();
    }

    protected override State OnUpdate()
    {
        if(m_VisionChecker != null)
        {
            var seesTarget = m_VisionChecker.CheckTargetInSight();
            if(m_Blackboard.TrySetBool(m_BlackboardProperty, seesTarget))
            {
                return State.Success;
            }
            else
            {
                Debug.LogWarning($"{m_ExecutorObject.name}'s tree blackboard doesn't have property with name {m_BlackboardProperty} or it's not bool");
            }
        }
        return State.Failure;
    }
}