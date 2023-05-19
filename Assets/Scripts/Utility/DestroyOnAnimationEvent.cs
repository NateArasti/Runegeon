using GabrielBigardi.SpriteAnimator.Runtime;
using UnityEngine;

[RequireComponent(typeof(SpriteAnimator))]
public class DestroyOnAnimationEvent : MonoBehaviour
{
    [SerializeField] private string m_EventName;
    [SerializeField] private float m_Delay;

    private void Awake()
    {
        GetComponent<SpriteAnimator>().AnimationEventCalled += OnAnimationEventCalled;
    }

    private void OnAnimationEventCalled(string eventName)
    {
        if(eventName == m_EventName)
        {
            Destroy(gameObject, m_Delay);
        }
    }
}
