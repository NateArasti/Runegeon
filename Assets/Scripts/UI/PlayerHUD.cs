using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
