using NaughtyAttributes;
using Pathfinding;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IVisionChecker, IChaser
{
    private const float k_FreeMoveSpaceRange = 0.5f;

    [BoxGroup("Chasing"), SerializeField] private float m_CloseRange = 0.5f;
    [BoxGroup("Chasing"), SerializeField] private float m_DistantRange = 2f;
    [BoxGroup("Chasing"), SerializeField] private float m_RotationSpeed = 10f;
    [Space]
    [Header("Editor Debug")]
    [SerializeField] private Transform m_PlayerTransformOverride;
    [SerializeField] private bool m_SeesPlayer;

    private AIDestinationSetter m_DestinationSetter;

    private Transform m_ChasePoint;
    private float m_CurrentChaseRange;
    private float m_CurrentRotateDirection = 1;

    private Vector3 m_PreviousPosition;

    private Transform Target => m_PlayerTransformOverride;

    public bool Chasing { get; set; }

    private float PositionDelta { get; set; }

    public bool AtTargetRange => 
        Mathf.Abs(Vector3.Distance(transform.position, Target.position) - m_CurrentChaseRange) < k_FreeMoveSpaceRange
        || PositionDelta <= 1e-7
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
            yield return new WaitForSeconds(1);
            m_CurrentRotateDirection = Random.value > 0.5f ? 1 : -1;
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
        PositionDelta = (transform.position - m_PreviousPosition).sqrMagnitude;
        m_PreviousPosition = transform.position;
    }

    public bool CheckTargetInSight()
    {
        return m_PlayerTransformOverride != null && m_SeesPlayer;
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
