using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class RunesContainer
{
    public static event UnityAction OnChange;

    private static readonly HashSet<IRuneEffect> s_StorageRuneEffects = new();
    private static readonly List<IRuneEffect> s_CurrentRuneEffects = new();

    public static IReadOnlyList<IRuneEffect> CurrentRuneEffects => s_CurrentRuneEffects;

    public static void ClaimRuneFromStorage(IRuneEffect runeEffect)
    {
        if (!s_StorageRuneEffects.Contains(runeEffect)) return;
        if (s_CurrentRuneEffects.Contains(runeEffect)) return;
        s_StorageRuneEffects.Remove(runeEffect);
        s_CurrentRuneEffects.Add(runeEffect);
        OnChange?.Invoke();
    }

    public static void DiscardRuneToStorage(IRuneEffect runeEffect)
    {
        if (!s_CurrentRuneEffects.Contains(runeEffect)) return;
        s_CurrentRuneEffects.Remove(runeEffect);
        s_StorageRuneEffects.Add(runeEffect);
        OnChange?.Invoke();
    }

    public static void AddRuneEffect(IRuneEffect runeEffect)
    {
        if (s_CurrentRuneEffects.Contains(runeEffect)) return;
        s_CurrentRuneEffects.Add(runeEffect);
        OnChange?.Invoke();
    }

    public static void RemoveRuneEffect(IRuneEffect runeEffect)
    {
        if (!s_CurrentRuneEffects.Contains(runeEffect)) return;
        s_CurrentRuneEffects.Remove(runeEffect);
        OnChange?.Invoke();
    }

    public static bool InStorage(IRuneEffect runeEffect) => s_StorageRuneEffects.Contains(runeEffect);

    public static void ClearAllRuneEffects()
    {
        s_CurrentRuneEffects.Clear();
        OnChange?.Invoke();
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
        runeEffect.OnApply(target);
    }

    public static void DiscardRuneFromTarget(IRuneEffect runeEffect, GameObject target)
    {
        runeEffect.OnDiscard(target);
    }
}
