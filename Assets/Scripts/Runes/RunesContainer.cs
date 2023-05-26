using System.Collections.Generic;
using UnityEngine;

public static class RunesContainer
{
    public static HashSet<IRuneEffect> StorageRuneEffects { get; private set; } = new();
    public static HashSet<IRuneEffect> CurrentRuneEffects { get; private set; } = new();

    public static void ClaimRuneFromStorage(IRuneEffect runeEffect)
    {
        AddRuneEffect(runeEffect);
        StorageRuneEffects.Remove(runeEffect);
    }

    public static void DiscardRuneToStorage(IRuneEffect runeEffect)
    {
        RemoveRuneEffect(runeEffect);
        StorageRuneEffects.Add(runeEffect);
    }

    public static void AddRuneEffect(IRuneEffect runeEffect)
    {
        CurrentRuneEffects.Add(runeEffect);
    }

    public static void RemoveRuneEffect(IRuneEffect runeEffect)
    {
        CurrentRuneEffects.Remove(runeEffect);
    }

    public static void ClearAllRuneEffects()
    {
        CurrentRuneEffects.Clear();
    }

    public static void ApplyAttackEffects(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        foreach (var effect in CurrentRuneEffects)
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
