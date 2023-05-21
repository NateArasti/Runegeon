public class ModificatedStats : IStats
{
    private IStats m_SourceStats;
    private float m_MoveSpeedModificator;
    private float m_AttackSpeedModificator;
    private float m_AttackDamageModificator;

    public ModificatedStats(IStats sourceStats,
        float moveSpeedModificator = 0,
        float attackSpeedModificator = 0,
        float attackDamageModificator = 0
        )
    {
        m_SourceStats = sourceStats;
        m_MoveSpeedModificator = moveSpeedModificator;
        m_AttackSpeedModificator = attackSpeedModificator;
        m_AttackDamageModificator = attackDamageModificator;
    }

    public int MaxHealth => m_SourceStats.MaxHealth;
    public float InvincibleTime => m_SourceStats.InvincibleTime;
    public float MoveSpeed => m_SourceStats.MoveSpeed + m_MoveSpeedModificator;
    public float AttackSpeed => m_SourceStats.AttackSpeed + m_AttackSpeedModificator;
    public float AttackDamage => m_SourceStats.AttackDamage + m_AttackDamageModificator;
}
