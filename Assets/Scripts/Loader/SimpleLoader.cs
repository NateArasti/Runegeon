using NaughtyAttributes;
using UnityEngine;

public class SimpleLoader : MonoBehaviour
{
    [SerializeField, Scene] private int m_Scene; 

    public void Load()
    {
        Loader.Load(m_Scene);
    }
}
