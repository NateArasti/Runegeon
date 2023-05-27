using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : DoubleStateObject
{
    [SerializeField] private InputActionProperty m_PauseToggleAction;

    private float m_PreviousTimeScale = 1;

    private void Awake()
    {
        m_PauseToggleAction.action.Enable();
        m_PauseToggleAction.action.performed += TogglePause;
    }

    protected override void Start()
    {
        base.Start();

        OnStateSet += OnPauseToggle;
    }

    private void OnDestroy()
    {
        OnStateSet -= OnPauseToggle;
        m_PauseToggleAction.action.performed -= TogglePause;
    }

    private void TogglePause(InputAction.CallbackContext obj)
    {
        ToggleState();
    }

    private void OnPauseToggle(bool paused)
    {
        if (paused)
        {
            m_PreviousTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = m_PreviousTimeScale;
        }
        DefaultButtonClickSFX.Instance.PlaySFX();
    }
}
