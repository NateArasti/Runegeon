using UnityEngine;
using UnityEngine.Events;

public class AttackProvider : MonoBehaviour, IAttackProvider
{
    [SerializeField] private float m_Damage = 1;
    [Space]
    [SerializeField] private UnityEvent<IAttackProvider, IAttackReciever> m_OnSuccessAttack;

    public float Damage => m_Damage;

    public void OnSuccessHit(IAttackReciever reciever)
    {
        m_OnSuccessAttack.Invoke(this, reciever);
    }
}
