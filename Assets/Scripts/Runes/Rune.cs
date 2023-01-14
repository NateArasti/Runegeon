using UnityEngine;

public class Rune : MonoBehaviour
{
    [SerializeField] private BaseRuneEffect m_Effect;

    public void AddEffectToPlayer()
    {
        if(m_Effect == null)
        {
            Debug.LogError("EFFECT IS NULL", this);
            return;
        }
        PlayerData.AddRuneEffect(m_Effect);
        Destroy(gameObject);
    }
}
