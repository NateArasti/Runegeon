using SimpleBehaviourTree;

public class ActivatePatrolNode : ActionNode
{
    private Patroller m_Patroller;

    public override string DisplayName => "Patrol On";

    protected override void OnStart()
    {
        m_Patroller = m_ExecutorObject.GetComponent<Patroller>();
    }

    protected override State OnUpdate()
    {
        if (m_Patroller == null) return State.Failure;
        m_Patroller.Patrolling = true;
        return State.Success;
    }
}