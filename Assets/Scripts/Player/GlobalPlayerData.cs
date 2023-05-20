using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
    private static GlobalPlayerData s_Instance;

    public static Transform PlayerTransform => s_Instance != null ? s_Instance.transform : null;

    private void Awake()
    {
        s_Instance = this;
    }
}
