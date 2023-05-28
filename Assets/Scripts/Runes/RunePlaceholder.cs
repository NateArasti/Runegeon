using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityExtensions;

public class RunePlaceholder : MonoBehaviour
{
    private readonly static HashSet<BaseRuneEffect> s_SpawnedRunes = new();

    [SerializeField] private BaseRuneEffect[] m_RuneVariants;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private UnityEvent m_OnConsume;
    private BaseRuneEffect m_ChosenEffect;

    private void Start()
    {
        var possibleRunes = new List<BaseRuneEffect>(m_RuneVariants);
        foreach (var rune in RunesContainer.CurrentRuneEffects)
        {
            if(rune is BaseRuneEffect runeEffect && possibleRunes.Contains(runeEffect))
                possibleRunes.Remove(runeEffect);
        }
        foreach (var rune in s_SpawnedRunes)
        {
            if (possibleRunes.Contains(rune))
                possibleRunes.Remove(rune);
        }
        var cleanedRunes = new List<BaseRuneEffect>(possibleRunes);
        foreach (var rune in possibleRunes)
        {
            if (RunesContainer.InStorage(rune))
                cleanedRunes.Remove(rune);
        }
        if(cleanedRunes.Count == 0)
        {
            CoroutineExtensions.InvokeSecondsDelayed(() => m_OnConsume.Invoke(), 1);
            Destroy(gameObject);
        }
        else
        {
            m_ChosenEffect = possibleRunes.GetRandomObject();
            s_SpawnedRunes.Add(m_ChosenEffect);
            m_SpriteRenderer.sprite = m_ChosenEffect.Icon;
        }
    }

    public void Consume()
    {
        if(m_ChosenEffect != null && GlobalPlayerData.PlayerTransform != null)
        {
            RunesContainer.AddRuneEffect(m_ChosenEffect);
            RunesContainer.ApplyRuneOnTarget(m_ChosenEffect, GlobalPlayerData.PlayerTransform.gameObject);
        }
        CoroutineExtensions.InvokeSecondsDelayed(() => m_OnConsume.Invoke(), 1);
        Destroy(gameObject);
    }
}
