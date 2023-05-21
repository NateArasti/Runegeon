using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class StatsContainer : MonoBehaviour
{
    [SerializeField] private DefaultStats m_DefaultStats;
    [Space]
    [SerializeField] private UnityEvent<int> m_OnMaxHealthSet;
    [SerializeField] private UnityEvent<float> m_OnInvincibleTimeSet;
    [SerializeField] private UnityEvent<float> m_OnMoveSpeedSet;
    [SerializeField] private UnityEvent<float> m_OnAttackSpeedSet;

    private IStats m_CurrentStats;
    public IStats CurrentStats
    {
        get => m_CurrentStats; private set
        {
            m_CurrentStats = value;
            ApplyStats();
        }
    }

    private void Start()
    {
        CurrentStats = m_DefaultStats;
    }

    public void SetModification(ModificatedStats modificatedStats)
    {
        CurrentStats = modificatedStats;
    }

    public void DiscardModifications()
    {
        CurrentStats = m_DefaultStats;
    }

    [Button]
    private void ApplyStats()
    {
        m_OnMaxHealthSet.Invoke(CurrentStats.MaxHealth);
        m_OnInvincibleTimeSet.Invoke(CurrentStats.InvincibleTime);
        m_OnMoveSpeedSet.Invoke(CurrentStats.MoveSpeed);
        m_OnAttackSpeedSet.Invoke(CurrentStats.AttackSpeed);

        foreach (var attackProvider in GetComponentsInChildren<AttackProvider>())
        {
            attackProvider.Damage = CurrentStats.AttackDamage;
        }
    }
}
