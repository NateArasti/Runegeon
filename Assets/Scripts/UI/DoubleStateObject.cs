using UnityEngine;
using UnityEngine.Events;

public class DoubleStateObject : MonoBehaviour
{
    public event UnityAction<bool> OnStateSet;

    [SerializeField] private bool m_StartState;
    [SerializeField] private UnityEvent m_OnEnable;
    [SerializeField] private UnityEvent m_OnDisable;
    private bool m_CurrentState;

    public bool CurrentState => m_CurrentState;

    protected virtual void Start()
    {
        ManualSetState(m_StartState);
    }

    public void ToggleState()
    {
        ManualSetState(!m_CurrentState);
    }

    public void ManualSetState(bool state)
    {
        m_CurrentState = state;
        if(m_CurrentState) m_OnEnable.Invoke();
        else m_OnDisable.Invoke();
        OnStateSet?.Invoke(m_CurrentState);
    }
}
