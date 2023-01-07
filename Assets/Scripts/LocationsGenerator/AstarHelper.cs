using UnityEngine;

[RequireComponent(typeof(AstarPath))]
public class AstarHelper : MonoBehaviour
{
    private AstarPath m_AstarPath;

    private void Awake()
    {
        m_AstarPath = GetComponent<AstarPath>();
    }

    public void GlobalRescan()
    {
        m_AstarPath.Scan();
    }
}
