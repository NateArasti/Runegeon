using NaughtyAttributes;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using UnityExtensions;

[RequireComponent(typeof(EnemyData), typeof(AIDestinationSetter))]
public class EnemyMovement : MonoBehaviour, IVisionChecker, IPatroller, IAttacker
{
    [SerializeField] private EnemyVisuals m_EnemyVisuals;

    [BoxGroup("Vision Check"), SerializeField] private float m_PlayerVisibleDistance = 5f;
    [BoxGroup("Attacks"), SerializeField] private float m_AttackDuration = 1f;
    [BoxGroup("Attacks"), SerializeField] private float m_AttackCooldown = 1f;
    [BoxGroup("Attacks"), SerializeField] private float m_AttackRange = 0.5f;
    [BoxGroup("Attacks"), SerializeField] private float m_AttackHeightThreshold = 0.1f;
    [BoxGroup("Attacks"), SerializeField] private UnityEvent<Vector2> m_OnAttack;

    private EnemyData m_EnemyData;
    private AIDestinationSetter m_DestinationSetter;
    private AIPath m_AIPath;

    private Vector2 m_LookVector = Vector2.right;
    private Vector3 m_PreviousPosition;

    private bool m_ForceNotTurning;

    public float AttackDuration => m_AttackDuration;
    public float AttackCooldown => m_AttackCooldown;

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
        m_EnemyData = GetComponent<EnemyData>();

        m_DestinationSetter = GetComponent<AIDestinationSetter>();
        m_AIPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        var delta = transform.position - m_PreviousPosition;
        m_LookVector = delta.x < 0 ? Vector2.left : Vector2.right;
        if (delta.x == 0) m_LookVector = Vector2.zero;

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
        return PlayerData.PlayerTransform != null &&
            Vector3.Distance(PlayerData.PlayerTransform.position, transform.position) < m_PlayerVisibleDistance;
    }

    public void SetNextTarget(bool forceSet = false)
    {
        if(!forceSet && m_DestinationSetter.target != null) return;
        SetPathfindingTarget(m_EnemyData.PatrolPoints.GetRandomObject());
    }

    public bool HasReachedPatrolDestination()
    {
        var check = m_DestinationSetter.target == null ||
            m_DestinationSetter.target == PlayerData.PlayerTransform ||
            Vector2.Distance(m_AIPath.destination, transform.position) < 0.01f;
        if (check) DiscardMovement();
        return check;
    }

    private void DiscardMovement()
    {
        m_DestinationSetter.target = null;
        m_AIPath.canMove = false;
    }

    public void GoToTarget()
    {
        SetPathfindingTarget(PlayerData.PlayerTransform);
    }

    public bool IsTargetInRange()
    {
        var check = Vector3.Distance(PlayerData.PlayerTransform.position, transform.position) < m_AttackRange &&
            Mathf.Abs(transform.position.y - PlayerData.PlayerTransform.position.y) < m_AttackHeightThreshold;
        if (check) DiscardMovement();
        return check;
    }

    public void StepBack(float stepBackTime)
    {
        StepBackTarget.position = transform.position + 
            (transform.position - PlayerData.PlayerTransform.position).normalized * 
            (stepBackTime * m_AIPath.maxSpeed);
        SetPathfindingTarget(StepBackTarget);
        m_ForceNotTurning = true;
        this.InvokeAfter(() => m_AIPath.reachedEndOfPath, DiscardMovement);
    }

    public void Attack()
    {
        m_EnemyVisuals.SwitchToState(EnemyVisuals.AnimationState.Attack);
        m_OnAttack.Invoke(m_LookVector);
        m_EnemyVisuals.InvokeSecondsDelayed(
            () => m_EnemyVisuals.SwitchToState(EnemyVisuals.AnimationState.IDLE), 
            AttackDuration
            );
    }
}
