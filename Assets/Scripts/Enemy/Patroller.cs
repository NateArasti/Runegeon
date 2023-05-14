using NaughtyAttributes;
using Pathfinding;
using UnityEngine;

public class Patroller : MonoBehaviour
{
    [SerializeField] private bool m_ChooseTargetsRandomly = true;
    [SerializeField, MinMaxSlider(0, 50)] private Vector2 m_DelayRange = new(1,5);

    private int m_Index;

    private IAstarAI m_Agent;
    private float m_SwitchTime = float.PositiveInfinity;

    public Transform[] Targets;

    public bool Patrolling { get; set; }

    private void Awake()
    {
        m_Agent = GetComponent<IAstarAI>();
    }

    private void Update()
    {
        if (Targets.Length == 0 || !Patrolling) return;

        bool search = false;

        // Note: using reachedEndOfPath and pathPending instead of reachedDestination here because
        // if the destination cannot be reached by the agent, we don't want it to get stuck, we just want it to get as close as possible and then move on.
        if (m_Agent.reachedEndOfPath && !m_Agent.pathPending && float.IsPositiveInfinity(m_SwitchTime))
        {
            m_SwitchTime = Time.time + Random.Range(m_DelayRange.x, m_DelayRange.y);
        }

        if (Time.time >= m_SwitchTime)
        {
            if (m_ChooseTargetsRandomly)
            {
                var newIndex = Random.Range(0, Targets.Length);
                if(newIndex == m_Index) // we don't want to set the same target twice
                {
                    newIndex++;
                    newIndex %= Targets.Length;
                }
                m_Index = newIndex;
            }
            else
            {
                m_Index++;
                m_Index %= Targets.Length;
            }

            search = true;
            m_SwitchTime = float.PositiveInfinity;
        }

        m_Agent.destination = Targets[m_Index].position;

        if (search) m_Agent.SearchPath();
    }
}
