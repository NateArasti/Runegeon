using UnityEngine;
using UnityEngine.Events;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance { get; private set; }

    [SerializeField] private UnityEvent m_OnRunesScreenForceOpen;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenRunesScreenWithStorage()
    {
        m_OnRunesScreenForceOpen.Invoke();
    }
}
