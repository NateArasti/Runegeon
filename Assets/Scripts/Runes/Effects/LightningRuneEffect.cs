using UnityEngine;

[CreateAssetMenu(fileName = "LightningEffect", menuName = "RuneEffects/LightningEffect")]
public partial class LightningRuneEffect : BaseRuneEffect
{
    [Space]
    [SerializeField, Range(0, 1)] private float m_LightningChance = 0.5f;
    [SerializeField] private LightningArcEffect m_LightningArcEffectPrefab;
    [SerializeField] private BounceAttack.BounceAttackData m_BounceAttackData;

    public override void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        if (Random.value > m_LightningChance) return;

        var newBounceAttack = new BounceAttack(m_BounceAttackData);
        newBounceAttack.OnAttack += OnAttackChain;
        newBounceAttack.ProvideAttack(attackReciever);
    }

    private void OnAttackChain(Vector3 firstPoint, Vector3 secondPoint)
    {
        var arcEffect = Instantiate(m_LightningArcEffectPrefab);

        arcEffect.SetEffect(
            firstPoint,
            secondPoint,
            m_BounceAttackData.BounceDelay,
            m_BounceAttackData.CollapseDelay);
    }
}
