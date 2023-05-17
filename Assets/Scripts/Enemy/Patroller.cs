using NaughtyAttributes;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class Patroller : MonoBehaviour
{
    [SerializeField] private bool m_ChooseTargetsRandomly = true;
    [SerializeField, MinMaxSlider(0, 50)] private Vector2 m_DelayRange = new(1, 5);
    [SerializeField, Min(0.1f)] private float m_StartDelay = 1;
    [Space]
    [SerializeField] private UnityEvent m_OnWalk;
    [SerializeField] private UnityEvent m_OnStay;

    private int m_Index;

    private IAstarAI m_Agent;
    private AIDestinationSetter m_DestinationSetter;
    private float m_SwitchTime = float.PositiveInfinity;

    public Transform[] Targets;
    private bool patrolling;

    public bool Patrolling
    {
        get => patrolling; 
        set
        {
            if (!value)
            {
                m_DestinationSetter.Target = null;
            }
            patrolling = value;
        }
    }

    private void Awake()
    {
        m_Agent = GetComponent<IAstarAI>();
        m_DestinationSetter = GetComponent<AIDestinationSetter>();
        m_SwitchTime = Time.time + m_StartDelay;
    }

    private void Update()
    {
        if (Targets.Length == 0 || !Patrolling) return;

        // Note: using reachedEndOfPath and pathPending instead of reachedDestination here because
        // if the destination cannot be reached by the agent, we don't want it to get stuck, we just want it to get as close as possible and then move on.
        if (m_Agent.reachedEndOfPath && !m_Agent.pathPending && float.IsPositiveInfinity(m_SwitchTime))
        {
            m_SwitchTime = Time.time + Random.Range(m_DelayRange.x, m_DelayRange.y);
            m_OnStay.Invoke();
        }

        if (Time.time >= m_SwitchTime)
        {
            if (m_ChooseTargetsRandomly)
            {
                var newIndex = Random.Range(0, Targets.Length);
                if (newIndex == m_Index) // we don't want to set the same target twice
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

            m_SwitchTime = float.PositiveInfinity;
            m_DestinationSetter.Target = Targets[m_Index];
            m_Agent.SearchPath();

            m_OnWalk.Invoke();
        }
    }
}
