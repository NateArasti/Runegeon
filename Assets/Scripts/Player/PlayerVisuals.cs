using BehavioursRectangularGraph;
using UnityEngine;
using UnityExtensions;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    private const string k_AttackTriggerPrefix = "Attack";

    private Animator m_Animator;

    private RectangularDirection m_PreviousDirection = RectangularDirection.Down;

    private readonly int m_FirstSwordAttackAnimationID = Animator.StringToHash("SwordAttack_1");
    private readonly int m_IDLEAnimationID = Animator.StringToHash("IDLE");
    private readonly int m_DodgeStateAnimationID = Animator.StringToHash("DodgeAnimation");
    private readonly int m_HorizontalAnimationID = Animator.StringToHash("Horizontal");
    private readonly int m_VerticalAnimationID = Animator.StringToHash("Vertical");
    private readonly int m_WalkingAnimationID = Animator.StringToHash("Walking");
    private readonly int m_DodgeAnimationID = Animator.StringToHash("Dodge");

    public bool CanDodge => m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash != m_DodgeStateAnimationID
            && m_Animator.GetNextAnimatorStateInfo(0).shortNameHash != m_DodgeStateAnimationID;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void TriggerAttackAnimation(int attackIndex)
    {
        if(attackIndex == 1)
        {
            m_Animator.CrossFade(m_FirstSwordAttackAnimationID, 0);
        }
        else
        {
            m_Animator.SetTrigger($"{k_AttackTriggerPrefix}{attackIndex}");
        }
    }

    public void Dodge(float dodgeDiscardDelay)
    {
        if (!CanDodge) return;
        m_Animator.CrossFade(m_IDLEAnimationID, 0);
        m_Animator.SetTrigger(m_DodgeAnimationID);
        this.InvokeSecondsDelayed(() => m_Animator.ResetTrigger(m_DodgeAnimationID), dodgeDiscardDelay);
    }

    public void ResetAttackTriggers(int attackCount)
    {
        for (var i = 2; i <= attackCount; i++)
            m_Animator.ResetTrigger($"{k_AttackTriggerPrefix}{i}");
    }

    public void AdjustMoveVisuals(Vector2 input, bool changeDirection)
    {
        var horizontalInput = input.x;
        var verticalInput = input.y;

        if (changeDirection)
        {
            HandleDirection(ref horizontalInput, ref verticalInput);

            var flip = horizontalInput > 0;
            var scale = transform.localScale;
            if (flip)
            {
                scale = new Vector3(Mathf.Abs(scale.x) * -1, scale.y, scale.z);
            }
            else
            {
                scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
            }
            transform.localScale = scale;

            m_Animator.SetFloat(m_HorizontalAnimationID, horizontalInput);
            m_Animator.SetFloat(m_VerticalAnimationID, verticalInput);
        }

        m_Animator.SetBool(m_WalkingAnimationID, input.sqrMagnitude > Mathf.Epsilon);
    }

    public void HandleDirection(ref float horizontalInput, ref float verticalInput)
    {
        if (horizontalInput == 0 && verticalInput == 0)
        {
            switch (m_PreviousDirection)
            {
                case RectangularDirection.Left:
                    horizontalInput = -1;
                    break;
                case RectangularDirection.Up:
                    verticalInput = 1;
                    break;
                case RectangularDirection.Right:
                    horizontalInput = 1;
                    break;
                case RectangularDirection.Down:
                    verticalInput = -1;
                    break;
            }
        }
        else
        {
            if(Mathf.Abs(horizontalInput) < Mathf.Abs(verticalInput))
            {
                if (verticalInput > 0) m_PreviousDirection = RectangularDirection.Up;
                else if (verticalInput < 0) m_PreviousDirection = RectangularDirection.Down;
            }
            else
            {
                if (horizontalInput > 0) m_PreviousDirection = RectangularDirection.Right;
                else if (horizontalInput < 0) m_PreviousDirection = RectangularDirection.Left;
            }
        }
    }
}
