using UnityEngine;
using UnityEngine.Events;

public class AttackProvider : MonoBehaviour, IAttackProvider
{
    [SerializeField] private int m_Damage = 0;
    [Space]
    public UnityEvent<IAttackProvider, IAttackReciever> OnSuccessAttack = new();

    public bool Active { get; set; } = true;

    public int Damage { get => m_Damage; set => m_Damage = value; }

    public void OnSuccessHit(IAttackReciever reciever)
    {
        if(!Active) return;
        OnSuccessAttack.Invoke(this, reciever);
    }
}
