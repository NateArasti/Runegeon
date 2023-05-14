using UnityEngine;

public class EnemyAI : MonoBehaviour, IVisionChecker
{
    [Header("Editor Debug")]
    [SerializeField] private Transform m_PlayerTransformOverride;
    [SerializeField] private bool m_SeesPlayer;

    public bool CheckTargetInSight()
    {
        return m_PlayerTransformOverride != null && m_SeesPlayer;
    }
}
