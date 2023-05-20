using System.Collections.Generic;
using UnityEngine;

public static class RunesContainer
{
    private static readonly HashSet<IRuneEffect> s_StorageRuneEffects = new();
    private static readonly HashSet<IRuneEffect> s_CurrentRuneEffects = new();

    public static IReadOnlyCollection<IRuneEffect> StorageRuneEffects => s_StorageRuneEffects;
    public static IReadOnlyCollection<IRuneEffect> CurrentRuneEffects => s_CurrentRuneEffects;

    public static void ClaimRuneFromStorage(IRuneEffect runeEffect)
    {
        if (!s_StorageRuneEffects.Contains(runeEffect))
        {
            Debug.LogWarning($"Can't claim rune({runeEffect}) that don't have");
            return;
        }

        AddRuneEffect(runeEffect);
        s_StorageRuneEffects.Remove(runeEffect);
    }

    public static void DiscardRuneToStorage(IRuneEffect runeEffect)
    {
        if (!s_CurrentRuneEffects.Contains(runeEffect))
        {
            Debug.LogWarning($"Can't discard rune({runeEffect}) that don't have");
            return;
        }

        RemoveRuneEffect(runeEffect);
        s_StorageRuneEffects.Add(runeEffect);
    }

    public static void AddRuneEffect(IRuneEffect runeEffect)
    {
        s_CurrentRuneEffects.Add(runeEffect);
    }

    public static void RemoveRuneEffect(IRuneEffect runeEffect)
    {
        s_CurrentRuneEffects.Remove(runeEffect);
    }

    public static void ClearAllRuneEffects()
    {
        s_CurrentRuneEffects.Clear();
    }

    public static void ApplyAttackEffects(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        foreach (var effect in s_CurrentRuneEffects)
        {
            effect.OnAttack(attackProvider, attackReciever);
        }
    }

    public static void ApplyRuneOnTarget(IRuneEffect runeEffect, GameObject target)
    {
        if (!s_CurrentRuneEffects.Contains(runeEffect))
        {
            Debug.LogWarning($"Can't apply rune({runeEffect}) that don't have");
            return;
        }

        runeEffect.OnApply(target);
    }

    public static void DiscardRuneFromTarget(IRuneEffect runeEffect, GameObject target)
    {
        if (!s_CurrentRuneEffects.Contains(runeEffect))
        {
            Debug.LogWarning($"Can't discard rune({runeEffect}) that don't have");
            return;
        }

        runeEffect.OnDiscard(target);
    }
}
