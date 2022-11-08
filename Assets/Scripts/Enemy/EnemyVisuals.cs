using NaughtyAttributes;
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
    [SerializeField] private AnimationStateKeys m_AnimationStateKeys;

    public AnimationState CurrentAnimationState { get; private set; } = AnimationState.IDLE;

    public void TurnToMoveVector(Vector2 lookDirection)
    {
        m_SpritePivot.localScale = m_SpritePivot.localScale
            .GetYZ(Mathf.Abs(m_SpritePivot.localScale.x) * Mathf.Sign(lookDirection.x));
    }

    public void SwitchToState(AnimationState state)
    {
        if (state == CurrentAnimationState) return;
        m_Animator.CrossFade(m_AnimationStateKeys[state], 0);
        CurrentAnimationState = state;
    }

    [System.Serializable]
    public class AnimationStateKeys : SerializedDictionary<AnimationState, string> { }
}
