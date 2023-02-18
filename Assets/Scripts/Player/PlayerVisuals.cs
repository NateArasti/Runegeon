using BehavioursRectangularGraph;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private RectangularDirection m_PreviousDirection = RectangularDirection.Down;

    public bool CanDodge => true;

    private void Awake()
    {
    }

    public void TriggerAttackAnimation(int attackIndex)
    {
    }

    public void Dodge(float dodgeDiscardDelay)
    {
    }

    public void ResetAttackTriggers(int attackCount)
    {
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
        }
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
