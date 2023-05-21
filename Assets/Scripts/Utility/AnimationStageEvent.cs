using GabrielBigardi.SpriteAnimator.Runtime;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteAnimator))]
public class AnimationStageEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> m_OnAnimationStage;
    [SerializeField] private string m_StartEventName = "start";
    [SerializeField] private string m_EndEventName = "end";

    private void Awake()
    {
        var animator = GetComponent<SpriteAnimator>();
        animator.AnimationEventCalled += AnimationEventCalled;
    }

    private void AnimationEventCalled(string eventName)
    {
        if(eventName == m_StartEventName)
        {
            m_OnAnimationStage.Invoke(true);
        }
        else if(eventName == m_EndEventName)
        {
            m_OnAnimationStage.Invoke(false);
        }
    }
}
