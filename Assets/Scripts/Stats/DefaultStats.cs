using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Stats")]
public class DefaultStats : ScriptableObject, IStats
{
    [SerializeField] private int m_MaxHealth = 12;
    [SerializeField] private float m_InvincibleTime = 1;
    [SerializeField] private float m_MoveSpeed = 4;
    [SerializeField] private float m_AttackSpeed = 1;
    [SerializeField] private int m_AttackDamage = 4;

    public int MaxHealth => m_MaxHealth;
    public float InvincibleTime => m_InvincibleTime;
    public float MoveSpeed => m_MoveSpeed;
    public float AttackSpeed => m_AttackSpeed;
    public int AttackDamage => m_AttackDamage;
}
