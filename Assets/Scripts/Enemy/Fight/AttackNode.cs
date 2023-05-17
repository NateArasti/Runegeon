using SimpleBehaviourTree;

public class AttackNode : ActionNode
{
    private IAttacker m_Attacker;

    public override string DisplayName => "Attack";

    protected override void OnStart()
    {
        m_Attacker = m_ExecutorObject.GetComponent<IAttacker>();
        m_Attacker.Attack();
    }

    protected override State OnUpdate()
    {
        if (m_Attacker == null) return State.Failure;
        return m_Attacker.Attacking ? State.Running : State.Success;
    }
}
