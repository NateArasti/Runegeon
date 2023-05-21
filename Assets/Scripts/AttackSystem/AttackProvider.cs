using UnityEngine;
using UnityEngine.Events;

public class AttackProvider : MonoBehaviour, IAttackProvider
{
    [SerializeField] private float m_Damage = 0;
    [Space]
    public UnityEvent<IAttackProvider, IAttackReciever> OnSuccessAttack = new();

    public bool Active { get; set; } = true;

    public float Damage { get => m_Damage; set => m_Damage = value; }

    private void Awake()
    {
        
    }

    public void OnSuccessHit(IAttackReciever reciever)
    {
        if(!Active) return;
        OnSuccessAttack.Invoke(this, reciever);
    }
}
