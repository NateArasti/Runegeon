using Pathfinding;
using UnityEngine;

public class AIDestinationSetter : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    private IAstarAI m_AI;

    public Transform Target { get => m_Target; set => m_Target = value; }

    private void OnEnable()
    {
        // Update the destination right before searching for a path as well.
        // This is enough in theory, but this script will also update the destination every
        // frame as the destination is used for debugging and may be used for other things by other
        // scripts as well. So it makes sense that it is up to date every frame.
        if (TryGetComponent<IAstarAI>(out m_AI)) m_AI.onSearchPath += Update;
    }

    private void OnDisable()
    {
        if (m_AI != null) m_AI.onSearchPath -= Update;
    }

    private void Update()
    {
        if (m_AI == null) return;

        if (Target != null)
        {
            if (m_AI.isStopped)
                m_AI.isStopped = false;
            m_AI.destination = Target.position;
        }
        else
        {
            m_AI.isStopped = true;
        }
    }
}
