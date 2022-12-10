using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtensions;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerVisuals m_PlayerVisuals;
    [SerializeField] private HealthSystem m_HealthSystem;
    [Header("Movement")]
    [SerializeField] private Joystick m_MoveJoystick;
    [SerializeField] private float m_MoveSpeed = 1;
    [SerializeField] private float m_DodgeSpeedIncrease = 1.5f;
    [SerializeField] private float m_DodgeDiscardDelay = 0.5f;
    [Header("Attacks")]
    [SerializeField] private float[] m_AttackTimings;
    [SerializeField] private float m_AttackSpeed = 1;
    [SerializeField] private float m_ComboCooldown = 1f;
    [Space]
    [Foldout("Actions"), SerializeField] private InputActionProperty m_MoveActionProperty;
    [Foldout("Actions"), SerializeField] private InputActionProperty m_AttackActionProperty;
    [Foldout("Actions"), SerializeField] private InputActionProperty m_DodgeActionProperty;
    private int m_CurrentAttackCount = 0;
    private float m_CurrentAttackCooldown;

    private float m_CurrentMoveSpeed;

    public bool Attacking => m_CurrentAttackCount > 0;

    private void Awake()
    {
        m_MoveActionProperty.action.Enable();
        m_AttackActionProperty.action.Enable();
        m_DodgeActionProperty.action.Enable();

        m_CurrentMoveSpeed = m_MoveSpeed;
    }

    private void OnDestroy()
    {
        m_MoveActionProperty.action.Disable();
        m_AttackActionProperty.action.Disable();
        m_DodgeActionProperty.action.Disable();
    }

    private void Start()
    {
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        while (true)
        {
            if(m_CurrentAttackCooldown <= 0)
            {
                m_CurrentAttackCount = 0;
                m_CurrentAttackCooldown = 0;
            }
            yield return null;
            m_CurrentAttackCooldown -= Time.deltaTime;
        }
    }

    private void Attack()
    {
        if(m_CurrentAttackCount < m_AttackTimings.Length)
        {
            if(m_CurrentAttackCount == 0)
            {
                DiscardAttackCombo();
                m_CurrentAttackCooldown = m_ComboCooldown;
            }
            m_CurrentAttackCount++;
            m_PlayerVisuals.TriggerAttackAnimation(m_CurrentAttackCount);
            m_CurrentAttackCooldown += m_AttackTimings[m_CurrentAttackCount - 1] / m_AttackSpeed;
        }
    }

    private void DiscardAttackCombo()
    {
        m_CurrentAttackCooldown = 0;
        m_CurrentAttackCount = 0;
        m_PlayerVisuals.ResetAttackTriggers(m_AttackTimings.Length);
    }

    private void Update()
    {
        if (m_DodgeActionProperty.action.triggered && m_PlayerVisuals.CanDodge)
        {
            m_HealthSystem.SetInvincible();
            DiscardAttackCombo();
            m_CurrentMoveSpeed = m_MoveSpeed * m_DodgeSpeedIncrease;
            this.InvokeSecondsDelayed(() => m_CurrentMoveSpeed = m_MoveSpeed, m_DodgeDiscardDelay);
            m_PlayerVisuals.Dodge(m_DodgeDiscardDelay);
        }
        else if (
#if UNITY_STANDALONE
            Input.GetMouseButtonDown(0) || 
#endif
            m_AttackActionProperty.action.triggered)
        {
            Attack();
        }

        var moveInput = m_MoveActionProperty.action.ReadValue<Vector2>() + m_MoveJoystick.Direction;
        m_PlayerVisuals.AdjustMoveVisuals(moveInput, !Attacking);

        if (!Attacking)
        {
            transform.Translate(m_CurrentMoveSpeed * Time.deltaTime * moveInput);
        }
    }
}
