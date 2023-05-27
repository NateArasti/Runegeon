using UnityEngine;

public abstract class BaseRuneEffect : ScriptableObject, IRuneEffect
{
    [SerializeField] private Sprite m_Icon;
    [SerializeField, TextArea] private string m_Description;

    public Sprite Icon => m_Icon;
    public string Description => m_Description;

    public virtual void OnApply(GameObject target) { }

    public virtual void OnDiscard(GameObject target) { }

    public virtual void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever) { }
}
