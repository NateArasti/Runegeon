using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float m_InvincibleTime = 1f;
    [SerializeField] private int m_MaxHealth = 100;
    [Space]
    [SerializeField] private UnityEvent m_OnHit;
    [SerializeField] private UnityEvent m_OnDeath;

    private bool m_Invincible;
    private float m_CurrentInvincibleCooldown;

    private int m_CurrentHealth;

    public int MaxHealth { get => m_MaxHealth; set => m_MaxHealth = value; }
    public int CurrentHealth => m_CurrentHealth;
    public float InvincibleTime { get => m_InvincibleTime; set => m_InvincibleTime = value; } 

    protected virtual void Awake()
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

    public void TryHit(IAttackProvider attackProvider)
    {
        if (m_Invincible) return;
        SetInvincible();
        m_CurrentHealth -= attackProvider.Damage;
        OnHealthChanged();
        m_OnHit.Invoke();
        if (m_CurrentHealth <= 0) m_OnDeath.Invoke();
    }

    public void SetInvincible(float invincibleTime = -1)
    {
        m_Invincible = true;
        m_CurrentInvincibleCooldown = invincibleTime == -1 ? m_InvincibleTime : invincibleTime;
    }

    [Button]
    private void TakeDamage()
    {
        if (m_Invincible) return;
        SetInvincible();
        m_CurrentHealth -= 10;
        OnHealthChanged();
        m_OnHit.Invoke();
        if (m_CurrentHealth <= 0) m_OnDeath.Invoke();
    }
}
