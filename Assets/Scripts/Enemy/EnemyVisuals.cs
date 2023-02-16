using QuickEye.Utility;
using UnityEngine;
using UnityExtensions;

public class EnemyVisuals : MonoBehaviour
{
    public enum AnimationState
    {
        IDLE,
        Walk,
        Death,
        Attack
    }

    [SerializeField] private Transform m_SpritePivot;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private UnityDictionary<AnimationState, string> m_AnimationStateKeys;

    public AnimationState CurrentAnimationState { get; private set; } = AnimationState.IDLE;

    public void TurnToMoveVector(Vector2 lookDirection)
    {
        if (lookDirection.x == 0) return;
        m_SpritePivot.localScale = m_SpritePivot.localScale
            .GetYZ(Mathf.Abs(m_SpritePivot.localScale.x) * Mathf.Sign(lookDirection.x));
    }

    public void Die()
    {
        SwitchToState(AnimationState.Death);
        this.InvokeSecondsDelayed(() => Destroy(gameObject), 2f);
    }

    public void SwitchToState(AnimationState state)
    {
        if (state == CurrentAnimationState) return;
        m_Animator.CrossFade(m_AnimationStateKeys[state], 0);
        CurrentAnimationState = state;
    }
}
