using UnityEngine;

[CreateAssetMenu(fileName = "DashEffect", menuName = "RuneEffects/DashEffect")]
public class DashRuneEffect : BaseRuneEffect
{
    private Dodger m_Dodger;
    private AttackProvider m_AttackProvider;

    public override void OnApply(GameObject target)
    {
        m_Dodger = target.GetComponentInChildren<Dodger>();
        if(m_Dodger == null) return;
        m_Dodger.dodgeType = Dodger.DodgeType.Dash;
        m_Dodger.OnDodgeStart += OnDashStart;
        m_Dodger.OnDodgeEnd += OnDashEnd;
        var hitbox = target.GetComponentInChildren<IAttackReciever>() as MonoBehaviour;
        m_AttackProvider = hitbox.gameObject.AddComponent<AttackProvider>();
        m_AttackProvider.OnSuccessAttack.AddListener(RunesContainer.ApplyAttackEffects);
        m_AttackProvider.Active = false;
    }

    private void OnDashStart()
    {
        m_AttackProvider.Active = true;
    }

    private void OnDashEnd()
    {
        m_AttackProvider.Active = false;
    }

    public override void OnDiscard(GameObject target)
    {
        if (m_Dodger == null) return;
        m_Dodger.dodgeType = Dodger.DodgeType.Roll;
        m_Dodger.OnDodgeStart -= OnDashStart;
        m_Dodger.OnDodgeEnd -= OnDashEnd;
        Destroy(m_AttackProvider);
    }
}
