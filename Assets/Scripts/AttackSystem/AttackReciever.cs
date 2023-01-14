using UnityEngine;
using UnityEngine.Events;

public class AttackReciever : MonoBehaviour, IAttackReciever
{
    [SerializeField] private UnityEvent<IAttackProvider> m_OnAttackHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<AttackProvider>(out var attackProvider))
        {
            RecieveAttack(attackProvider);
        }
    }

    public void RecieveAttack(IAttackProvider attackProvider)
    {
        m_OnAttackHit.Invoke(attackProvider);
        attackProvider.OnSuccessHit(this);
    }
}
