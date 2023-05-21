using GabrielBigardi.SpriteAnimator.Runtime;
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
    [SerializeField] private SpriteAnimator m_SpriteAnimator;
    [SerializeField] private float m_DodgeDistance = 1;
    [SerializeField] private float m_DodgeCooldown = 1;
    [Header("Roll")]
    [SerializeField] private SpriteAnimation m_RollAnimation;
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

    public void Dodge(Vector2 dodgeDirection, out float dodgeTime)
    {
        dodgeTime = 0;

        if (m_CurrentDodgeCooldown > 0) return;

        switch (dodgeType)
        {
            case DodgeType.Roll:
                Roll(dodgeDirection, out dodgeTime);
                break;
            case DodgeType.Dash:
                Dash(dodgeDirection, out dodgeTime);
                break;
            default:
                break;
        }
        m_CurrentDodgeCooldown = m_DodgeCooldown;
        OnDodgeStart?.Invoke();
        this.InvokeSecondsDelayed(() => OnDodgeEnd?.Invoke(), dodgeTime);
    }

    private void Roll(Vector2 dodgeDirection, out float dodgeTime)
    {
        m_SpriteAnimator.PlayIfNotPlaying(m_RollAnimation);
        var targetRollTime = m_DodgeDistance / MoveSpeed;
        m_RollAnimation.FPS = (int) (m_RollAnimation.Frames.Count / targetRollTime);
        dodgeTime = m_RollAnimation.GetAnimationTime();
        transform.DashMove(dodgeDirection, m_DodgeDistance, dodgeTime);
    }
    
    private void Dash(Vector2 dodgeDirection, out float dodgeTime)
    {
        dodgeTime = m_DashTime;

        m_SpriteAnimator.Pause();
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

        transform.DashMove(dodgeDirection, m_DodgeDistance, dodgeTime);

        this.InvokeSecondsDelayed(() =>
        {
            m_SpriteRenderer.color = cachedColor;
            m_SpriteAnimator.Play();
        }, dodgeTime);
    }
}
