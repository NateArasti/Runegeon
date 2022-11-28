using UnityEngine;
using UnityEngine.Events;

public class InteractionBase : MonoBehaviour
{
    [SerializeField] protected UnityEvent<InteractionBase> m_OnEnter;
    [SerializeField] protected UnityEvent<InteractionBase> m_OnExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<InteractionBase>(out var interaction))
        {
            m_OnEnter.Invoke(interaction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<InteractionBase>(out var interaction))
        {
            m_OnExit.Invoke(interaction);
        }
    }
}
