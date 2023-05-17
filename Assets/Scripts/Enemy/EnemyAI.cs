using GabrielBigardi.SpriteAnimator.Runtime;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityExtensions;

public class EnemyAI : MonoBehaviour, IVisionChecker, IChaser, IAttacker
{
    private const float k_FreeMoveSpaceRange = 0.5f;
    [BoxGroup("Life Cycle"), SerializeField] private SpriteAnimation m_DeathAnimation;
    [BoxGroup("Life Cycle"), SerializeField] private UnityEvent<SpriteAnimation> m_OnDeath;

    [BoxGroup("Chasing"), SerializeField] private float m_CloseRange = 0.5f;
    [BoxGroup("Chasing"), SerializeField] private float m_DistantRange = 2f;
    [BoxGroup("Chasing"), SerializeField] private float m_RotationDirectionCheckDelta = 1f;
    [BoxGroup("Chasing"), SerializeField] private float m_RotationSpeed = 10f;
    [BoxGroup("Chasing"), SerializeField] private bool m_TryToStayAtSameY = true;
    [BoxGroup("Chasing"), SerializeField] private UnityEvent m_OnStartChasing;
    [BoxGroup("Chasing"), SerializeField] private UnityEvent m_OnStopChasing;

    [BoxGroup("Attack"), SerializeField] private SpriteAnimation m_AttackAnimation;
    [BoxGroup("Attack"), SerializeField] private UnityEvent<SpriteAnimation> m_OnAttack;

    [Foldout("Debug"), SerializeField] private Transform m_PlayerTransformOverride;
    [Foldout("Debug"), SerializeField] private bool m_SeesPlayer;

    private AIDestinationSetter m_DestinationSetter;

    private Transform m_ChasePoint;
    private float m_CurrentChaseRange;
    private float m_CurrentRotateDirection = 1;

    private Vector3 m_PreviousPosition;

    private bool m_Chasing;
    public bool Chasing
    {
        get => m_Chasing;
        set
        {
            if(m_Chasing != value)
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

    private float PositionDeltaMagnitude { get; set; }

    private Transform Target => m_PlayerTransformOverride;

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
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_RotationDirectionCheckDelta);
            m_CurrentRotateDirection = Random.value > 0.5f ? 1 : -1;

            if (!m_TryToStayAtSameY) continue;

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
    }

    private void Update()
    {
        if (Chasing)
        {
            var shift = Quaternion.Euler(0, 0, m_CurrentRotateDirection * Time.deltaTime * m_RotationSpeed) * (transform.position - Target.position);
            shift = shift.normalized * m_CurrentChaseRange;

            m_ChasePoint.position = Target.position + shift;

            m_DestinationSetter.Target = m_ChasePoint;
        }
    }

    private void LateUpdate()
    {
        var delta = transform.position - m_PreviousPosition;
        PositionDeltaMagnitude = delta.sqrMagnitude;

        if (Mathf.Abs(delta.x) > 1e-3)
        {
            int lookDirection;
            if (Chasing)
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

    public void Die()
    {
        m_DestinationSetter.Target = null;
        enabled = false;
        m_OnDeath.Invoke(m_DeathAnimation);
        CoroutineExtensions.InvokeSecondsDelayed(() => Destroy(gameObject), m_DeathAnimation.Frames.Count / m_DeathAnimation.FPS);
    }

    public bool CheckTargetInSight()
    {
        return m_SeesPlayer && m_PlayerTransformOverride != null;
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

    public void Attack()
    {
        Attacking = true;
        m_OnAttack.Invoke(m_AttackAnimation);
        this.InvokeSecondsDelayed(() => Attacking = false, m_AttackAnimation.Frames.Count / m_AttackAnimation.FPS);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Chasing)
        {
            Gizmos.DrawWireSphere(Target.position, m_CurrentChaseRange);
        }
    }
#endif
}
