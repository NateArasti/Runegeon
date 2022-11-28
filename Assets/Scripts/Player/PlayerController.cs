using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_MoveSpeed = 1;
    [Space]
    [Foldout("Actions"), SerializeField] private InputActionProperty m_MoveActionProperty;
    [Space]
    [Foldout("Events"), SerializeField] private UnityEvent<Vector2> m_OnMoveInput;

    private void Awake()
    {
        m_MoveActionProperty.action.Enable();
    }

    private void Update()
    {
        var moveInput = m_MoveActionProperty.action.ReadValue<Vector2>();
        transform.Translate(m_MoveSpeed * Time.deltaTime * moveInput);

        m_OnMoveInput.Invoke(moveInput);
    }
}
