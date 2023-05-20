using UnityEngine;
using UnityEngine.Events;

public class AttackReciever : MonoBehaviour, IAttackReciever
{
    [SerializeField] private UnityEvent<IAttackProvider> m_OnAttackHit;

    public bool Active { get; set; } = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!Active) return;
        if(collision.gameObject.TryGetComponent<IAttackProvider>(out var attackProvider) && 
            attackProvider.Active)
        {
            RecieveAttack(attackProvider);
        }
    }

    public void RecieveAttack(IAttackProvider attackProvider)
    {
        if (!Active) return;
        m_OnAttackHit.Invoke(attackProvider);
        attackProvider.OnSuccessHit(this);
    }
}
