using DG.Tweening;
using UnityEngine;
using UnityExtensions;

public class DashMove : MonoBehaviour
{
    [SerializeField] private Vector2 m_MoveOffset = Vector2.right;
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private float m_Delay = 0.25f;

    public void Dash(Vector2 direction)
    {
        if (direction == Vector2.zero) direction = Vector2.right;
        var destination = transform.position + 
            new Vector3(direction.x * m_MoveOffset.x, direction.y * m_MoveOffset.y);
        this.InvokeSecondsDelayed(() => transform.DOMove(destination, m_Duration), m_Delay);
    }
}
