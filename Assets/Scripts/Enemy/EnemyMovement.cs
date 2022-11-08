using NaughtyAttributes;
using Pathfinding;
using UnityEngine;
using UnityExtensions;

[RequireComponent(typeof(AIDestinationSetter))]
public class EnemyMovement : MonoBehaviour, IVisionChecker, IPatroller, IAttacker
{
    [SerializeField] private EnemyVisuals m_EnemyVisuals;
    [SerializeField] private Transform m_Player;

    [BoxGroup("Patrol"), SerializeField] private Transform[] m_PatrolTargets;

    [BoxGroup("Vision Check"), SerializeField] private float m_PlayerVisibleDistance = 5f;

    [BoxGroup("Attacks"), SerializeField] private float m_AttakDuration = 1f;
    [BoxGroup("Attacks"), SerializeField] private float m_AttakCooldown = 1f;
    [BoxGroup("Attacks"), SerializeField] private float m_AttakRange = 0.5f;
    [BoxGroup("Attacks"), SerializeField] private float m_StepBackDistance;

    private AIDestinationSetter m_DestinationSetter;
    private AIPath m_AIPath;

    private Vector2 m_LookVector = Vector2.right;
    private Vector3 m_PreviousPosition;

    private bool m_ForceNotTurning;

    public float AttackDuration => m_AttakDuration;
    public float AttackCooldown => m_AttakCooldown;

    private Transform m_StepBackTarget;
    public Transform StepBackTarget 
    { 
        get 
        { 
            if(m_StepBackTarget == null)
            {
                var newGO = new GameObject($"Stepback target for {name}");
                newGO.hideFlags = HideFlags.HideAndDontSave;
                m_StepBackTarget = newGO.transform;
            }
            return m_StepBackTarget; 
        }
    }

    private void Awake()
    {
        m_DestinationSetter = GetComponent<AIDestinationSetter>();
        m_AIPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        var delta = transform.position - m_PreviousPosition;
        m_LookVector = delta.x < 0 ? Vector2.left : Vector2.right;
        if (!m_ForceNotTurning)
            m_EnemyVisuals.TurnToMoveVector(m_LookVector);

        m_PreviousPosition = transform.position;

        if (m_DestinationSetter.target != null &&
            m_EnemyVisuals.CurrentAnimationState == EnemyVisuals.AnimationState.IDLE)
        {
            m_EnemyVisuals.SwitchToState(EnemyVisuals.AnimationState.Walk);
        }
        else if (m_DestinationSetter.target == null &&
            m_EnemyVisuals.CurrentAnimationState == EnemyVisuals.AnimationState.Walk)
        {
            m_EnemyVisuals.SwitchToState(EnemyVisuals.AnimationState.IDLE);
        }
    }

    private void SetPathfindingTarget(Transform newTarget)
    {
        m_DestinationSetter.target = newTarget;
        m_AIPath.SearchPath();
        m_ForceNotTurning = false;
        m_AIPath.canMove = true;
    }

    public bool CheckTargetInSight()
    {
        return m_Player != null &&
            Vector3.Distance(m_Player.position, transform.position) < m_PlayerVisibleDistance;
    }

    public void SetNextTarget(bool forceSet = false)
    {
        if(!forceSet && m_DestinationSetter.target != null) return;
        var newTargetIndex = Random.Range(0, m_PatrolTargets.Length);
        SetPathfindingTarget(m_PatrolTargets[newTargetIndex]);
    }

    public bool HasReachedDestination(float remainingDistance = 0)
    {
        var check = m_AIPath.remainingDistance <= remainingDistance + Mathf.Epsilon;
        if (check)
        {
            m_DestinationSetter.target = null;
            m_AIPath.canMove = false;
        }
        return check;
    }

    public void GoToTarget()
    {
        SetPathfindingTarget(m_Player);
    }

    public bool IsTargetInRange()
    {
        var check = Vector3.Distance(m_Player.position, transform.position) < m_AttakRange;
        if (check)
        {
            m_DestinationSetter.target = null;
            m_AIPath.canMove = false;
        }
        return check;
    }

    public void StepBack()
    {
        StepBackTarget.position = transform.position + 
            (transform.position - m_Player.position).normalized * m_StepBackDistance;
        SetPathfindingTarget(StepBackTarget);
        m_ForceNotTurning = true;
    }

    public void Attack()
    {
        m_EnemyVisuals.SwitchToState(EnemyVisuals.AnimationState.Attack);
        m_EnemyVisuals.InvokeSecondsDelayed(
            () => m_EnemyVisuals.SwitchToState(EnemyVisuals.AnimationState.IDLE), 
            AttackDuration
            );
    }
}
