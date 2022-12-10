using UnityEngine;
using UnityEngine.Events;

public class AttackReciever : MonoBehaviour
{
    [SerializeField] private UnityEvent<AttackProvider> m_OnAttackHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<AttackProvider>(out var attackProvider))
        {
            m_OnAttackHit.Invoke(attackProvider);
        }
    }
}
