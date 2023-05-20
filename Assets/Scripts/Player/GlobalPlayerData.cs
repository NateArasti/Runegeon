using NaughtyAttributes;
using System.Data;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
    private static GlobalPlayerData s_Instance;

    [SerializeField] private GameObject m_Player;
    [SerializeField] private BaseRuneEffect m_AutoAddRune;

    public static Transform PlayerTransform => s_Instance != null ? 
        s_Instance.m_Player.transform : null;

    private void Awake()
    {
        s_Instance = this;

        RunesContainer.AddRuneEffect(m_AutoAddRune);
    }

    private void Start()
    {
        //foreach (var rune in RunesContainer.CurrentRuneEffects)
        //{
        //    RunesContainer.ApplyRuneOnTarget(rune, m_Player);
        //}
    }

    [Button]
    private void ApplyRune()
    {
        RunesContainer.ApplyRuneOnTarget(m_AutoAddRune, m_Player);
    }

    [Button]
    private void DiscardRune()
    {
        RunesContainer.DiscardRuneFromTarget(m_AutoAddRune, m_Player);
    }
}
