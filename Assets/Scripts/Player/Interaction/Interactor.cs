using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : InteractionBase
{
    public event UnityAction OnInteract;

    [Space]
    [SerializeField] private InputActionProperty m_InteractiAction;

    private void Start()
    {
        m_InteractiAction.action.Enable();
        m_InteractiAction.action.performed += Interact;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke();
    }
}
