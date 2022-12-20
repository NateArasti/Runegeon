using UnityEngine;

public class Perfomance : MonoBehaviour
{
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 30;
        GUILayout.Label($"FPS: {1f / Time.smoothDeltaTime}");
    }
}
