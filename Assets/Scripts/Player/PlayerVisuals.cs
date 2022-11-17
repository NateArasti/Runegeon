using BehavioursRectangularGraph;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;

    private readonly int m_HorizontalAnimationID = Animator.StringToHash("Horizontal");
    private readonly int m_VerticalAnimationID = Animator.StringToHash("Vertical");
    private readonly int m_WalkingAnimationID = Animator.StringToHash("Walking");

    private RectangularDirection m_PreviousDirection = RectangularDirection.Down;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AdjustVisuals(Vector2 input)
    {
        var horizontalInput = input.x;
        var verticalInput = input.y;

        HandleDirection(ref horizontalInput, ref verticalInput);

        m_SpriteRenderer.flipX = horizontalInput > 0;

        m_Animator.SetFloat(m_HorizontalAnimationID, horizontalInput);
        m_Animator.SetFloat(m_VerticalAnimationID, verticalInput);

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
            if (verticalInput > 0) m_PreviousDirection = RectangularDirection.Up;
            if (verticalInput < 0) m_PreviousDirection = RectangularDirection.Down;
            if (horizontalInput > 0) m_PreviousDirection = RectangularDirection.Right;
            if (horizontalInput < 0) m_PreviousDirection = RectangularDirection.Left;
        }
    }
}
