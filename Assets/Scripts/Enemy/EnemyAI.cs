using GabrielBigardi.SpriteAnimator.Runtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityExtensions;

public class EnemyAI : MonoBehaviour, IVisionChecker, IChaser, IAttacker
{
    private const float k_FreeMoveSpaceRange = 0.5f;

    [BoxGroup("Life Cycle"), SerializeField] private SpriteAnimation m_DeathAnimation;
    [BoxGroup("Life Cycle"), SerializeField] private UnityEvent<SpriteAnimation> m_OnDeath;
    private bool m_IsAlive = true;

    [BoxGroup("Vision Check"), SerializeField] private float m_VisionRange;
    [BoxGroup("Vision Check"), SerializeField] private bool m_CheckForAll;
    [BoxGroup("Vision Check"), SerializeField, ShowIf(nameof(m_CheckForAll))] private LayerMask m_VisionLayerMask;

    [BoxGroup("Attack"), SerializeField] private SpriteAnimator m_SpriteAnimator;
    [BoxGroup("Attack"), SerializeField] private SpriteAnimation m_AttackAnimation;
    [BoxGroup("Attack"), SerializeField] private string m_AttackTriggerStartEvent = "attack_start";
    [BoxGroup("Attack"), SerializeField] private string m_AttackTriggerEndEvent = "attack_end";
    [BoxGroup("Attack"), SerializeField] private UnityEvent<bool> m_OnAttackTrigger;

    #region Chasing

    [BoxGroup("Chasing"), SerializeField] private float m_CloseRange = 0.5f;
    [BoxGroup("Chasing"), SerializeField] private float m_DistantRange = 2f;
    [BoxGroup("Chasing"), SerializeField] private float m_RotationDirectionCheckDelta = 1f;
    [BoxGroup("Chasing"), SerializeField] private float m_RotationSpeed = 10f;
    [BoxGroup("Chasing"), SerializeField] private bool m_TryToStayAtSameY = true;
    [BoxGroup("Chasing"), SerializeField] private UnityEvent m_OnStartChasing;
    [BoxGroup("Chasing"), SerializeField] private UnityEvent m_OnStopChasing;

    private Transform m_ChasePoint;
    private float m_CurrentChaseRange;
    private float m_CurrentRotateDirection = 1;
    private float m_CurrentRotationCheckTime;

    private bool m_Chasing;
    public bool Chasing
    {
        get => m_Chasing;
        set
        {
            if (m_Chasing != value)
            {
                if (value)
                {
                    m_OnStartChasing.Invoke();
                }
                else
                {
                    m_OnStopChasing.Invoke();
                }
            }
            m_Chasing = value;
        }
    }

    #endregion

    private AIDestinationSetter m_DestinationSetter;
    private Transform m_CurrentColliderTarget;
    private Vector3 m_PreviousPosition;

    private float PositionDeltaMagnitude { get; set; }

    private Transform Target => m_CheckForAll ? m_CurrentColliderTarget : PlayerData.PlayerTransform;

    public bool Attacking { get; private set; }

    public bool AtTargetRange =>
        Mathf.Abs(Vector3.Distance(transform.position, Target.position) - m_CurrentChaseRange) < k_FreeMoveSpaceRange
        || PositionDeltaMagnitude <= 1e-7
        ;

    private void Awake()
    {
        m_DestinationSetter = GetComponent<AIDestinationSetter>();

        m_ChasePoint = new GameObject("Chase Point").transform;
        m_ChasePoint.gameObject.hideFlags = HideFlags.HideAndDontSave;

        m_SpriteAnimator.AnimationEventCalled += HandleAnimationEvent;
    }

    private void HandleAnimationEvent(string eventName)
    {
        if (!m_IsAlive) return;

        if (eventName.Equals(m_AttackTriggerStartEvent))
        {
            m_OnAttackTrigger.Invoke(true);
        }
        else if (eventName.Equals(m_AttackTriggerEndEvent))
        {
            m_OnAttackTrigger.Invoke(false);
        }
    }

    private void Update()
    {
        if (!m_IsAlive) return;
        if (Chasing) Chase();
    }

    private void LateUpdate()
    {
        if (!m_IsAlive) return;
        var delta = transform.position - m_PreviousPosition;
        PositionDeltaMagnitude = delta.sqrMagnitude;

        if (Mathf.Abs(delta.x) > 1e-3)
        {
            int lookDirection;
            if (Chasing || Attacking)
            {
                lookDirection = (Target.position - transform.position).x > 0 ? 1 : -1;
            }
            else
            {
                lookDirection = delta.x > 0 ? 1 : -1;
            }
            transform.localScale = new Vector3(lookDirection, 1, 1);
        }

        m_PreviousPosition = transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (Chasing)
            {
                Gizmos.DrawWireSphere(Target.position, m_CurrentChaseRange);
            }
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, m_CloseRange);
            Gizmos.DrawWireSphere(transform.position, m_DistantRange);
        }
    }
#endif

    public bool CheckTargetInSight()
    {
        if (!m_IsAlive) return false;

        if (m_CheckForAll)
        {
            m_CurrentColliderTarget = null;

            var targets = Physics2D.OverlapCircleAll(transform.position, m_VisionRange, m_VisionLayerMask);
            foreach(var target in targets)
            {
                var root = target.transform.root;
                if (root.gameObject == gameObject ||
                    !root.TryGetComponent<EnemyAI>(out var _)) continue;
                m_CurrentColliderTarget = target.transform.root;
                break;
            }

            return m_CurrentColliderTarget != null;
        }

        return PlayerData.PlayerTransform != null && 
            Vector3.Distance(PlayerData.PlayerTransform.position, transform.position) <= m_VisionRange;
    }

    public void Die()
    {
        if (!m_IsAlive) return;
        m_DestinationSetter.Target = null;
        m_IsAlive = false;
        m_OnDeath.Invoke(m_DeathAnimation);
        CoroutineExtensions.InvokeSecondsDelayed(() => Destroy(gameObject), m_DeathAnimation.Frames.Count / m_DeathAnimation.FPS);
    }

    #region Chasing

    private void Chase()
    {
        var shift = Quaternion.Euler(0, 0, m_CurrentRotateDirection * Time.deltaTime * m_RotationSpeed) * (transform.position - Target.position);
        shift = shift.normalized * m_CurrentChaseRange;

        m_ChasePoint.position = Target.position + shift;

        m_DestinationSetter.Target = m_ChasePoint;

        m_CurrentRotationCheckTime -= Time.deltaTime;
        if (m_CurrentRotationCheckTime <= 0)
        {
            m_CurrentRotationCheckTime = m_RotationDirectionCheckDelta;
            CheckRotation();
        }
    }

    private void CheckRotation()
    {
        m_CurrentRotateDirection = Random.value > 0.5f ? 1 : -1;

        if (!m_TryToStayAtSameY) return;

        if ((transform.position.x > Target.position.x && transform.position.y < Target.position.y)
            ||
            (transform.position.x < Target.position.x && transform.position.y > Target.position.y))
        {
            m_CurrentRotateDirection = 1;
        }
        else if ((transform.position.x > Target.position.x && transform.position.y > Target.position.y)
            ||
            (transform.position.x < Target.position.x && transform.position.y < Target.position.y))
        {
            m_CurrentRotateDirection = -1;
        }
    }

    public void StayAtRange(IChaser.ChaseRange chaseRange)
    {
        switch (chaseRange)
        {
            case IChaser.ChaseRange.Close:
                m_CurrentChaseRange = m_CloseRange;
                break;
            case IChaser.ChaseRange.Distant:
                m_CurrentChaseRange = m_DistantRange;
                break;
            default:
                m_CurrentChaseRange = 0;
                break;
        }
    }

    #endregion

    #region Attack

    public void Attack()
    {
        if (!m_IsAlive) return;
        Attacking = true;
        m_SpriteAnimator.Play(m_AttackAnimation);
        this.InvokeSecondsDelayed(() => Attacking = false, m_AttackAnimation.Frames.Count / m_AttackAnimation.FPS);
    }

    #endregion
}