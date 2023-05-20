using UnityEngine;
using UnityEngine.Events;

public class AttackProvider : MonoBehaviour, IAttackProvider
{
    [SerializeField] private float m_Damage = 0;
    [Space]
    [SerializeField] private UnityEvent<IAttackProvider, IAttackReciever> m_OnSuccessAttack = new();

    public bool Active { get; set; } = true;

    public float Damage => m_Damage;

    public void OnSuccessHit(IAttackReciever reciever)
    {
        if(!Active) return;
        m_OnSuccessAttack.Invoke(this, reciever);
    }
}
