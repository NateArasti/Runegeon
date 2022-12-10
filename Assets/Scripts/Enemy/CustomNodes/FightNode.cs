using NaughtyAttributes;
using SimpleBehaviourTree;
using UnityEngine;

public class FightNode : ActionNode
{
    [SerializeField, MinMaxSlider(0.5f, 2f)] private Vector2 m_WaitApplier;

    private IAttacker m_Attacker;
    private float m_AttackDuration;
    private float m_AttackCooldown;

    public override string DisplayName => "Fight";

    protected override void OnStart()
    {
        m_Attacker = m_ExecutorObject.GetComponent<IAttacker>();
        m_Attacker.GoToTarget();
    }

    protected override State OnUpdate()
    {
        if (m_Attacker == null) return State.Failure;

        if (m_AttackDuration > 0)
        {
            m_AttackDuration -= Time.deltaTime;
            if(m_AttackDuration <= 0)
            {
                m_AttackCooldown = m_Attacker.AttackCooldown * 
                    Random.Range(m_WaitApplier.x, m_WaitApplier.y);
                m_Attacker.StepBack(m_AttackCooldown);
            }
            return State.Running;
        }

        if (m_AttackCooldown > 0)
        {
            m_AttackCooldown -= Time.deltaTime;
            if(m_AttackCooldown <= 0)
            {
                m_Attacker.GoToTarget();
            }
            return State.Running;
        }

        if (m_Attacker.IsTargetInRange())
        {
            m_Attacker.Attack();
            m_AttackDuration = m_Attacker.AttackDuration;
        }

        return State.Running;
    }

}