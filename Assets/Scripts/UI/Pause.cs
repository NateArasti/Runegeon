using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private InputActionProperty m_PauseToggleAction;
    [Space]
    [SerializeField] private Button m_PauseButton;
    [Space]
    [SerializeField] private UnityEvent m_OnPause;
    [SerializeField] private UnityEvent m_OnPlay;

    private float m_PreviousTimeScale;

    private void Awake()
    {
        m_PauseToggleAction.action.Enable();
        m_PauseToggleAction.action.performed += TogglePause;
    }

    private void TogglePause(InputAction.CallbackContext obj)
    {
        if (m_PauseButton.gameObject.activeInHierarchy)
        {
            m_PreviousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            m_OnPause.Invoke();
        }
        else
        {
            Time.timeScale = m_PreviousTimeScale;
            m_OnPlay.Invoke();
        }
    }
}
