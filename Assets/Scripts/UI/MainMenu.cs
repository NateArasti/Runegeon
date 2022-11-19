using NaughtyAttributes;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Scene, SerializeField] private int m_PlayLoadScene;

    public void Play()
    {
        Loader.Load(m_PlayLoadScene);
    }
}
