using NaughtyAttributes;
using SimpleBehaviourTree;
using UnityEngine;

public class PatrolNode : ActionNode
{
    [SerializeField, MinMaxSlider(0, 5)] private Vector2 m_WaitDuration;
    [SerializeField, Min(0)] private float m_ReachedDistance;
    private IPatroller m_Patroller;
    private float m_CurrentWaitDuration;

    public override string DisplayName => "Patrol";

    private bool Waiting => m_CurrentWaitDuration > 0;

    protected override void OnStart()
    {
        m_Patroller = m_ExecutorObject.GetComponent<IPatroller>();
        m_Patroller.SetNextTarget(true);
        m_CurrentWaitDuration = 0;
    }

    protected override State OnUpdate()
    {
        if(m_Patroller == null) return State.Failure;

        if (!Waiting && m_Patroller.HasReachedPatrolDestination())
        {
            m_CurrentWaitDuration = Random.Range(m_WaitDuration.x, m_WaitDuration.y);
        }

        if(Waiting)
        {
            m_CurrentWaitDuration -= Time.deltaTime;
            if (!Waiting)
            {
                m_Patroller.SetNextTarget(true);
            }
        }

        return State.Running;
    }
}