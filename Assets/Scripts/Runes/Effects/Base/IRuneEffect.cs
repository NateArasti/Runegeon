using UnityEngine;

public interface IRuneEffect
{
    void OnApply(GameObject target);

    void OnDiscard(GameObject target);

    void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever);
}
