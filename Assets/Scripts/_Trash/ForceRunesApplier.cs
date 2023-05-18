using NaughtyAttributes;
using UnityEngine;

public class ForceRunesApplier : MonoBehaviour
{
    [SerializeField] private EffectApplier m_EffectApplier;
    [SerializeField] private BaseRuneEffect[] m_Runes;

    private void Awake()
    {
        SetRunes();
    }

    [Button]
    private void SetRunes()
    {
        m_EffectApplier.ClearAllRuneEffects();
        m_EffectApplier.AddRuneEffects(m_Runes);
    }
}
