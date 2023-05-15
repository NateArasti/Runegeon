using SimpleBehaviourTree;
using UnityEngine;

public class PatrolNode : ActionNode
{
    private Patroller m_Patroller;

    public override string DisplayName => "Patrol";

    protected override void OnStart()
    {
        if (m_ExecutorObject.TryGetComponent<Patroller>(out m_Patroller))
            m_Patroller.Patrolling = true;
        Debug.Log(m_Patroller);
    }

    protected override void OnStop()
    {
        if (m_Patroller != null)
            m_Patroller.Patrolling = false;
    }

    protected override State OnUpdate()
    {
        if (m_Patroller == null) return State.Failure;
        return State.Running;
    }
}