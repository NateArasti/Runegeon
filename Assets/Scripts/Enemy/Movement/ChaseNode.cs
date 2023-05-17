using SimpleBehaviourTree;
using UnityEngine;

public class ChaseNode : ActionNode
{
    [SerializeField] private IChaser.ChaseRange m_ChaseRange;
    [SerializeField] private float m_TimeInRange;
    private IChaser m_Chaser;
    private float m_CurrentWaitTime;

    public override string DisplayName => $"{m_ChaseRange} Chase";

    protected override void OnStart()
    {
        m_Chaser = m_ExecutorObject.GetComponent<IChaser>();
        m_Chaser.StayAtRange(m_ChaseRange);
        m_Chaser.Chasing = true;

        m_CurrentWaitTime = m_TimeInRange;
    }

    protected override void OnStop()
    {
        if(m_Chaser != null)
            m_Chaser.Chasing = false;
    }

    protected override State OnUpdate()
    {
        if(m_Chaser == null) return State.Failure;

        if (m_Chaser.AtTargetRange)
        {
            m_CurrentWaitTime -= Time.deltaTime;
            if(m_CurrentWaitTime <= 0)
            {
                return State.Success;
            }
        }

        return State.Running;
    }
}