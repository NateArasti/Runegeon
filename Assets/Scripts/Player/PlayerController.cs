using BehavioursRectangularGraph;
using GabrielBigardi.SpriteAnimator.Runtime;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtensions;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        IDLE,
        Move,
        Dodge,
        Attack
    }

    [Header("References")]
    [SerializeField] private Transform m_PlayerCamera;
    [SerializeField] private HealthSystem m_HealthSystem;
    [SerializeField] private SpriteAnimator m_SpriteAnimator;
    [SerializeField] private Dodger m_Dodger;

    [Header("Movement")]
    [SerializeField] private Joystick m_MoveJoystick;
    [SerializeField] private SpriteAnimation m_IDLEAnimation;
    [SerializeField] private SpriteAnimation m_MoveAnimation;

    [Header("Attacks")]
    [SerializeField] private SpriteAnimation[] m_AttackComboAnimations;

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

        transform.localScale = new Vector3(m_LookDirection == RectangularDirection.Left ? -1 : 1, 1, 1);
    }

    private void HandleMovement()
    {
        var moveInput = MoveInput;
        if (moveInput.sqrMagnitude > 0)
        {
            transform.Translate(MoveSpeed * Time.deltaTime * moveInput);
            m_SpriteAnimator.PlayIfNotPlaying(m_MoveAnimation);
            m_CurrentState = PlayerState.Move;
        }
        else
        {
            m_CurrentState = PlayerState.IDLE;
            m_SpriteAnimator.PlayIfNotPlaying(m_IDLEAnimation);
        }
    }

    #region Attack

    private void Attack(InputAction.CallbackContext obj)
    {
        if (m_CurrentState == PlayerState.Dodge ||
            m_AttackComboAnimations.Length == 0) return;

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
            if (m_CurrentComboIndex < m_AttackComboAnimations.Length - 1)
            {
                m_CurrentComboIndex++;
                m_SpriteAnimator.PlayIfNotPlaying(m_AttackComboAnimations[m_CurrentComboIndex]);
                var attackAnimation = m_AttackComboAnimations[m_CurrentComboIndex];
                var normalFPS = attackAnimation.FPS;
                attackAnimation.FPS = (int)(normalFPS * AttackSpeed);
                yield return new WaitForSeconds(attackAnimation.GetAnimationTime());
                attackAnimation.FPS = normalFPS;
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
            m_Dodger.Dodge(
                BehavioursRectangularGraph.Utility.GetCorrespondingVector(m_LookDirection),
                out dodgeTime);
        }

        m_HealthSystem.SetInvincible(dodgeTime);

        this.InvokeSecondsDelayed(
            () => m_CurrentState = PlayerState.IDLE,
            dodgeTime
            );
    }
}
