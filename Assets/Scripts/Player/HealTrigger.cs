using UnityEngine;

public class HealTrigger : MonoBehaviour
{
    private bool m_Healed;

    public void HealPlayer()
    {
        if(m_Healed || GlobalPlayerData.PlayerTransform == null) return;
        if (GlobalPlayerData.PlayerTransform.TryGetComponent<PlayerHealthSystem>(out var healthSystem))
        {
            m_Healed = true;
            healthSystem.Heal();
        }
    }
}
