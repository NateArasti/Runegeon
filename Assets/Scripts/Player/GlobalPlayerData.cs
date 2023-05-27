using NaughtyAttributes;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
    private static GlobalPlayerData s_Instance;

    [SerializeField] private GameObject m_Player;
    [SerializeField] private BaseRuneEffect[] m_StartRunes;
    [SerializeField] private BaseRuneEffect m_Rune;

    public static Transform PlayerTransform => s_Instance != null ? 
        s_Instance.m_Player.transform : null;

    private void Awake()
    {
        s_Instance = this;

        foreach (var rune in m_StartRunes)
        {
            RunesContainer.AddRuneEffect(rune);
        }
    }

    private void Start()
    {
        foreach (var rune in RunesContainer.CurrentRuneEffects)
        {
            RunesContainer.ApplyRuneOnTarget(rune, m_Player);
        }
    }

    [Button]
    private void AddRune()
    {
        RunesContainer.AddRuneEffect(m_Rune);
    }

    [Button]
    private void RemoveRune()
    {
        RunesContainer.RemoveRuneEffect(m_Rune);
    }

    [Button]
    private void ApplyRune()
    {
        RunesContainer.ApplyRuneOnTarget(m_Rune, m_Player);
    }

    [Button]
    private void DiscardRune()
    {
        RunesContainer.DiscardRuneFromTarget(m_Rune, m_Player);
    }

    [Button]
    private void ClaimRuneFromStorage()
    {
        RunesContainer.ClaimRuneFromStorage(m_Rune);
    }

    [Button]
    private void DiscardRuneToStorage()
    {
        RunesContainer.DiscardRuneToStorage(m_Rune);
    }
}
