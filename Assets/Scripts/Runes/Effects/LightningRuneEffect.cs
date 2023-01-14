using UnityEngine;
using UnityExtensions;

[CreateAssetMenu(fileName = "LightningEffect", menuName = "RuneEffects/LightningEffect")]
public class LightningRuneEffect : BaseRuneEffect
{
    [SerializeField] private BounceAttackData m_BounceAttackData;

    public override void OnAttack(IAttackReciever attackReciever)
    {
        var newBounceAttack = new BounceAttack(m_BounceAttackData);
        newBounceAttack.ProvideAttack(attackReciever);
    }

    #region Bounce

    [System.Serializable]
    private class BounceAttackData
    {
        [SerializeField] private LightningArcEffect m_LightningArcEffectPrefab;
        [SerializeField] private float m_BounceDelay = 0.5f;
        [SerializeField] private int m_MaxBounceCount = 3;
        [SerializeField] private float m_BounceRange = 1f;
        [SerializeField] private float m_BounceDamage = 10f;
        [SerializeField] private LayerMask m_BounceMask;

        public LightningArcEffect LightningArcEffectPrefab => m_LightningArcEffectPrefab;
        public float BounceDelay => m_BounceDelay;
        public int MaxBounceCount => m_MaxBounceCount;
        public float BounceRange => m_BounceRange;
        public float BounceDamage => m_BounceDamage;
        public LayerMask BounceMask => m_BounceMask;
    }

    private class BounceAttack : IAttackProvider
    {
        private readonly BounceAttackData m_AttackData;

        private int m_CurrentBounceCount;
        private LightningArcEffect m_CurrentArcEffect;

        public float Damage => m_AttackData.BounceDamage;

        public BounceAttack(BounceAttackData attackData)
        {
            m_CurrentBounceCount = 0;
            m_CurrentArcEffect = null;

            m_AttackData = attackData;
        }

        public void OnSuccessHit(IAttackReciever reciever)
        {
            if (m_CurrentBounceCount >= m_AttackData.MaxBounceCount) return;
            CoroutineExtensions.InvokeSecondsDelayed(
                () => ProvideAttack(reciever),
                m_AttackData.BounceDelay
                );
        }

        public void ProvideAttack(IAttackReciever previousReciever)
        {
            if (previousReciever == null) return;

            IAttackReciever nextReciever = null;

            var center = previousReciever is MonoBehaviour behaviour ?
                behaviour.transform.position :
                Vector3.zero;
            var closeTargets =
                Physics2D.OverlapCircleAll(center, m_AttackData.BounceRange, m_AttackData.BounceMask);
            foreach (var possibleTarget in closeTargets)
            {
                var possibleReciever = possibleTarget.GetComponent<IAttackReciever>();
                if(possibleReciever != null && previousReciever != possibleReciever)
                {
                    nextReciever = possibleReciever;
                    break;
                }
            }

            if (nextReciever == null) return;

            m_CurrentBounceCount++;
            Debug.Log($"Bouncing on {nextReciever}", nextReciever as MonoBehaviour);

            var previousRecieverPosition = (previousReciever as MonoBehaviour).transform.position;
            var nextRecieverPosition = (nextReciever as MonoBehaviour).transform.position;

            if(m_CurrentArcEffect == null)
            {
                m_CurrentArcEffect = Instantiate(m_AttackData.LightningArcEffectPrefab);
            }

            m_CurrentArcEffect.SetEffect(
                previousRecieverPosition,
                nextRecieverPosition,
                m_AttackData.BounceDelay,
                0.75f * m_AttackData.BounceDelay);

            nextReciever.RecieveAttack(this);
        }
    }

    #endregion
}
