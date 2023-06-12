using BehavioursRectangularGraph;
using GabrielBigardi.SpriteAnimator.Runtime;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtensions;
using QuickEye.Utility;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        IDLE,
        Move,
        Dodge,
        Attack
    }

    [Header("References")]
    [SerializeField] private Transform m_PlayerCamera;
    [SerializeField] private HealthSystem m_HealthSystem;
    [SerializeField] private Dodger m_Dodger;
    [SerializeField] private PlayerVisuals m_PlayerVisuals;

    [Header("Movement")]
    [SerializeField] private bool m_InvertVisualsOnXAxis = false;
    [SerializeField] private Joystick m_MoveJoystick;

    [Foldout("Actions"), SerializeField] private InputActionProperty m_MoveActionProperty;
    [Foldout("Actions"), SerializeField] private InputActionProperty m_AttackActionProperty;
    [Foldout("Actions"), SerializeField] private InputActionProperty m_DodgeActionProperty;

    private int m_CurrentComboIndex = -1;
    private bool m_AutoTriggerNextAttack;

    private PlayerState m_CurrentState = PlayerState.IDLE;
    private RectangularDirection m_LookDirection = RectangularDirection.Right;

    public float AttackSpeed { get; set; } = 1;

    public float MoveSpeed { get; set; } = 1;

    private Vector2 MoveInput => m_MoveJoystick.Direction + m_MoveActionProperty.action.ReadValue<Vector2>();

    private void Awake()
    {
        m_MoveActionProperty.action.Enable();
        m_AttackActionProperty.action.Enable();
        m_DodgeActionProperty.action.Enable();

        m_AttackActionProperty.action.performed += Attack;
        m_DodgeActionProperty.action.performed += Dodge;
    }

    private void Start()
    {
        InitializeAttackProviders();
        m_PlayerCamera.parent = null;
    }

    private void OnDestroy()
    {
        m_AttackActionProperty.action.performed -= Attack;
        m_DodgeActionProperty.action.performed -= Dodge;

        if(m_PlayerCamera != null && m_PlayerCamera.gameObject != null)
            Destroy(m_PlayerCamera.gameObject);
    }

    private void Update()
    {
        HandleLookDirection();

        if (m_CurrentState == PlayerState.IDLE || m_CurrentState == PlayerState.Move)
            HandleMovement();
    }

    public void Die()
    {
        var currentRuneEffects = new List<IRuneEffect>(RunesContainer.CurrentRuneEffects);
        foreach (var rune in currentRuneEffects)
        {
            RunesContainer.DiscardRuneFromTarget(rune, gameObject);
            RunesContainer.RemoveRuneEffect(rune);
        }
        enabled = false;
    }

    private void HandleLookDirection()
    {
        var moveInput = MoveInput;

        if(moveInput.sqrMagnitude == 0 || 
            m_CurrentState == PlayerState.Attack ||
            m_CurrentState == PlayerState.Dodge) return;

        if (Mathf.Abs(MoveInput.y) > Mathf.Abs(MoveInput.x))
        {
            m_LookDirection = MoveInput.y >= 0 ? 
                RectangularDirection.Up : 
                RectangularDirection.Down;
        }
        else
        {
            m_LookDirection = MoveInput.x >= 0 ?
                RectangularDirection.Right :
                RectangularDirection.Left;
        }

        if(m_InvertVisualsOnXAxis)
            transform.localScale = new Vector3(m_LookDirection == RectangularDirection.Left ? -1 : 1, 1, 1);
    }

    private void HandleMovement()
    {
        var moveInput = MoveInput;
        if (moveInput.sqrMagnitude > 0)
        {
            transform.Translate(MoveSpeed * Time.deltaTime * moveInput);
            m_CurrentState = PlayerState.Move;
            m_PlayerVisuals.PlayWalkAnimation(m_LookDirection);
        }
        else
        {
            m_CurrentState = PlayerState.IDLE;
            m_PlayerVisuals.PlayIDLEAnimation(m_LookDirection);
        }
    }

    #region Attack

    private void Attack(InputAction.CallbackContext obj)
    {
        if (m_CurrentState == PlayerState.Dodge ||
            m_PlayerVisuals.AttackComboCount == 0) return;

        m_AutoTriggerNextAttack = true;
        if (m_CurrentState != PlayerState.Attack)
        {
            m_CurrentComboIndex = -1;
            StartCoroutine(AttackComboRoutine());
        }
        m_CurrentState = PlayerState.Attack;
    }

    private IEnumerator AttackComboRoutine()
    {
        yield return null;
        while (m_AutoTriggerNextAttack)
        {
            if (m_CurrentState != PlayerState.Attack)
            {
                yield break;
            }
            m_AutoTriggerNextAttack = false;
            if (m_CurrentComboIndex < m_PlayerVisuals.AttackComboCount - 1)
            {
                m_CurrentComboIndex++;
                var attackAnimation = m_PlayerVisuals.PlayAttackAnimation(m_CurrentComboIndex, m_LookDirection, 1 / AttackSpeed);
                if(attackAnimation != null)
                    yield return new WaitForSeconds(attackAnimation.GetAnimationTime());
            }
        }
        if (m_CurrentState == PlayerState.Attack)
        {
            m_CurrentState = PlayerState.IDLE;
        }
    }

    private void InitializeAttackProviders()
    {
        var attackProviders = GetComponentsInChildren<AttackProvider>(true);
        foreach (var attackProvider in attackProviders)
        {
            attackProvider.OnSuccessAttack.AddListener(RunesContainer.ApplyAttackEffects);
        }
    }

    #endregion

    private void Dodge(InputAction.CallbackContext obj)
    {
        if (m_CurrentState == PlayerState.Dodge) return;
        m_CurrentState = PlayerState.Dodge;

        var dodgeTime = 0f;
        if (m_Dodger != null)
        {
            m_Dodger.Dodge(m_LookDirection, out dodgeTime);
        }

        m_HealthSystem.SetInvincible(dodgeTime);

        this.InvokeSecondsDelayed(
            () => m_CurrentState = PlayerState.IDLE,
            dodgeTime
            );
    }
}
