using UnityEngine;

public abstract class BaseRuneEffect : ScriptableObject, IRuneEffect
{
    public virtual void OnAttack(IAttackReciever attackReciever) { }

    public virtual void OnHit() { }

    public virtual void OnMove() { }

    public virtual void OnRoll() { }
}
