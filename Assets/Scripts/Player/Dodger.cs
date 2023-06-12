using BehavioursRectangularGraph;
using System;
using UnityEngine;
using UnityExtensions;

public class Dodger : MonoBehaviour
{
    public enum DodgeType
    {
        Roll,
        Dash
    }

    public event Action OnDodgeStart;
    public event Action OnDodgeEnd;

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float m_DodgeDistance = 1;
    [SerializeField] private float m_DodgeCooldown = 1;
    [SerializeField] private LayerMask m_LayerMask;
    [Header("Roll")]
    [SerializeField] private PlayerVisuals m_PlayerVisuals;
    [Header("Dash")]
    [SerializeField] private GameObject m_DashEffect;
    [SerializeField] private float m_DashTime = 0.1f;
    [SerializeField, Range(0, 1)] private float m_SpriteAlpha = 0.3f;
    private float m_CurrentDodgeCooldown;
    public DodgeType dodgeType = DodgeType.Roll;

    public float MoveSpeed { get; set; } = 1;

    private void Update()
    {
        if(m_CurrentDodgeCooldown > 0)
        {
            m_CurrentDodgeCooldown -= Time.deltaTime;
        }
    }

    public void Dodge(RectangularDirection dodgeDirection, out float dodgeTime)
    {
        dodgeTime = 0;

        if (m_CurrentDodgeCooldown > 0) return;
        switch (dodgeType)
        {
            case DodgeType.Roll:
                Roll(dodgeDirection, out dodgeTime);
                break;
            case DodgeType.Dash:
                var convertedDirection = 
                    BehavioursRectangularGraph.Utility.GetCorrespondingVector(dodgeDirection);
                Dash(convertedDirection, out dodgeTime);
                break;
            default:
                break;
        }
        m_CurrentDodgeCooldown = m_DodgeCooldown;
        OnDodgeStart?.Invoke();
        this.InvokeSecondsDelayed(() => OnDodgeEnd?.Invoke(), dodgeTime);
    }

    private void Roll(RectangularDirection dodgeDirection, out float dodgeTime)
    {
        var targetRollTime = m_DodgeDistance / MoveSpeed;
        var rollAnimation = m_PlayerVisuals.PlayDodgeAnimation(dodgeDirection, targetRollTime);
        if (rollAnimation != null)
        {
            dodgeTime = rollAnimation.GetAnimationTime();
        }
        else
        {
            dodgeTime = 1;
        }
        var convertedDirection =
            BehavioursRectangularGraph.Utility.GetCorrespondingVector(dodgeDirection);
        transform.DashMove(convertedDirection, AdjustDodgeDistance(convertedDirection), dodgeTime);
    }

    private float AdjustDodgeDistance(Vector2 dodgeDirection)
    {
        var hit = Physics2D.Raycast(transform.position, dodgeDirection, m_DodgeDistance, m_LayerMask);
        var distance = hit.collider != null ? hit.distance * 0.8f : m_DodgeDistance;
        if (distance < 0.5f)
        {
            distance = 0;
        }
        return distance;
    }
    
    private void Dash(Vector2 dodgeDirection, out float dodgeTime)
    {
        dodgeTime = m_DashTime;

        var effect = Instantiate(m_DashEffect, transform.position, Quaternion.identity);
        if(dodgeDirection.x >= 0)
        {
            var scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }

        var cachedColor = m_SpriteRenderer.color;
        var newColor = cachedColor;
        newColor.a = m_SpriteAlpha; 
        m_SpriteRenderer.color = newColor;

        transform.DashMove(dodgeDirection, AdjustDodgeDistance(dodgeDirection), dodgeTime);

        this.InvokeSecondsDelayed(() =>
        {
            m_SpriteRenderer.color = cachedColor;
        }, dodgeTime);
    }
}
