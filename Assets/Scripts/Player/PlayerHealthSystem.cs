using UnityEngine;

public class PlayerHealthSystem : HealthSystem
{
    [Space]
    [SerializeField] private HeartsHealthBar m_HealthBar;

    protected override void Awake()
    {
        m_HealthBar.SetMaxHealth(MaxHealth);
        base.Awake();
    }

    protected override void OnHealthChanged()
    {
        m_HealthBar.SetCurrentHealth(CurrentHealth);
    }
}
