using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    private readonly List<IRuneEffect> m_RuneEffects = new();

    public void AddRuneEffect(IRuneEffect runeEffect)
    {
        m_RuneEffects.Add(runeEffect);
    }

    public void AddRuneEffects(IEnumerable<IRuneEffect> runeEffects)
    {
        m_RuneEffects.AddRange(runeEffects);
    }

    public void ClearAllRuneEffects()
    {
        m_RuneEffects.Clear();
    }

    public void ApplyAttackEffects(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        foreach (var effect in m_RuneEffects)
        {
            effect.OnAttack(attackProvider, attackReciever);
        }
    }

    public void ApplyMoveEffects()
    {
        foreach (var effect in m_RuneEffects)
        {
            effect.OnMove();
        }
    }

    public void ApplyHitEffects()
    {
        foreach (var effect in m_RuneEffects)
        {
            effect.OnHit();
        }
    }

    public void ApplyRollEffects()
    {
        foreach (var effect in m_RuneEffects)
        {
            effect.OnRoll();
        }
    }
}
