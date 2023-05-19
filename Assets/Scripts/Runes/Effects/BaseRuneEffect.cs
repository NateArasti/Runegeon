using UnityEngine;

public abstract class BaseRuneEffect : ScriptableObject, IRuneEffect
{
    public virtual void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever) { }

    public virtual void OnHit() { }

    public virtual void OnMove() { }

    public virtual void OnRoll() { }
}
