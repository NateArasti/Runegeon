using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : InteractionBase
{
    [Space]
    [SerializeField] private bool m_Locked;
    [SerializeField, HideIf(nameof(m_Locked))] protected UnityEvent m_OnInteraction;

    private void Awake()
    {
        m_OnEnter.AddListener(HandleEnter);
        m_OnExit.AddListener(HandleExit);
    }

    private void OnDestroy()
    {
        m_OnEnter.RemoveListener(HandleEnter);
        m_OnExit.RemoveListener(HandleExit);
    }

    private void HandleEnter(InteractionBase interaction)
    {
        if(!m_Locked && interaction is Interactor interactor)
        {
            interactor.OnInteract += m_OnInteraction.Invoke;
        }
    }

    private void HandleExit(InteractionBase interaction)
    {
        if (!m_Locked && interaction is Interactor interactor)
        {
            interactor.OnInteract -= m_OnInteraction.Invoke;
        }
    }
}
