using UnityEngine;
using UnityEngine.Events;

public class ManualPlayerTeleport : MonoBehaviour
{
    [SerializeField] private Transform m_TeleportPoint;
    [SerializeField] private UnityEvent m_OnTeleport;

    public void Teleport()
    {
        if (GlobalPlayerData.PlayerTransform == null) return;
        GlobalPlayerData.PlayerTransform.position = m_TeleportPoint.position;
        m_OnTeleport.Invoke();
    }
}
