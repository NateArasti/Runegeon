using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionEffect", menuName = "RuneEffects/ExplosionEffect")]
public class ExplosionRuneEffect : BaseRuneEffect
{
    [SerializeField] private GameObject m_EffectPrefab;
    [SerializeField] private float m_ExplosionRadius = 2;
    [SerializeField] private float m_ExplosionDamage = 1;
    [SerializeField] private float m_ExplosionForce = 1;
    [SerializeField] private float m_MoveTime = 0.2f;
    [SerializeField] private LayerMask m_ExplosionLayerMask;

    public override void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        var center = attackProvider is MonoBehaviour behaviour ?
            behaviour.transform.position :
            Vector3.zero;

        var closeTargets =
            Physics2D.OverlapCircleAll(center, m_ExplosionRadius, m_ExplosionLayerMask);

        var explosionAttack = new ExplosionAttackProvider(m_ExplosionDamage);

        Instantiate(m_EffectPrefab, center, Quaternion.identity);

        foreach (var target in closeTargets)
        {
            if(target.TryGetComponent<IAttackReciever>(out var reciever))
            {
                var targetRoot = target.transform.root;
                var direction = targetRoot.position - center;
                if(Mathf.Approximately(direction.sqrMagnitude, 0))
                {
                    direction = Random.insideUnitCircle;
                }

                direction.Normalize();

                targetRoot.DOMove(targetRoot.position + direction * m_ExplosionForce, m_MoveTime);

                reciever.RecieveAttack(explosionAttack);
            }
        }
    }

    private class ExplosionAttackProvider : IAttackProvider
    {
        public ExplosionAttackProvider(float damage)
        {
            Damage = damage;
        }

        public float Damage { get; private set; }

        public void OnSuccessHit(IAttackReciever reciever) { }
    }
}