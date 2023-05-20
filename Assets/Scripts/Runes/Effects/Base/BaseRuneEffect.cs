using UnityEngine;

public abstract class BaseRuneEffect : ScriptableObject, IRuneEffect
{
    public virtual void OnApply(GameObject target) { }

    public virtual void OnDiscard(GameObject target) { }

    public virtual void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever) { }
}
