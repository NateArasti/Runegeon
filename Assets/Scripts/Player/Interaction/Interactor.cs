using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : InteractionBase
{
    public event UnityAction OnInteract;

    [Space]
    [SerializeField] private InputActionProperty m_InteractAction;

    private void Start()
    {
        m_InteractAction.action.Enable();
        m_InteractAction.action.performed += Interact;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke();
    }
}
