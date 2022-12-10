using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float m_InvincibleTime = 1f;
    [SerializeField] private float m_MaxHealth = 100;
    [Space]
    [SerializeField] private UnityEvent m_OnHit;
    [SerializeField] private UnityEvent m_OnDeath;

    private bool m_Invincible;
    private float m_CurrentInvincibleCooldown;

    private float m_CurrentHealth;

    protected float MaxHealth => m_MaxHealth; 
    protected float CurrentHealth => m_CurrentHealth; 

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
        OnHealthChanged();
        StartCoroutine(InvincibleCooldown());
    }

    private IEnumerator InvincibleCooldown()
    {
        while (true)
        {
            if (m_CurrentInvincibleCooldown <= 0)
            {
                m_Invincible = false;
                m_CurrentInvincibleCooldown = 0;
            }
            yield return null;
            m_CurrentInvincibleCooldown -= Time.deltaTime;
        }
    }

    protected virtual void OnHealthChanged() { }

    public void TryHit(AttackProvider attackProvider)
    {
        if (m_Invincible) return;
        SetInvincible();
        m_CurrentHealth -= attackProvider.Damage;
        OnHealthChanged();
        m_OnHit.Invoke();
        if (m_CurrentHealth < 0) m_OnDeath.Invoke();
    }

    public void SetInvincible()
    {
        m_Invincible = true;
        m_CurrentInvincibleCooldown = m_InvincibleTime;
    }
}
