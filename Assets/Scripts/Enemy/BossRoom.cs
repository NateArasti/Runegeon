using UnityEngine;
using UnityEngine.Events;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> m_OnBlockSet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out var _))
        {
            m_OnBlockSet.Invoke(true);
        }
    }

    public void RemoveBlock()
    {
        m_OnBlockSet.Invoke(false);
        Destroy(gameObject);
    }
}
