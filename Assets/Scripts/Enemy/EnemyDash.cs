using UnityEngine;

public class EnemyDash : MonoBehaviour
{
    [SerializeField] private EnemyAI m_EnemyAI;
    [SerializeField] private float m_DashDistance = 1;
    [SerializeField] private float m_DashTime = 0.1f;

    public void Dash(bool started)
    {
        if (started)
        {
            var dodgeDirection = m_EnemyAI.Target.position - transform.position;
            transform.DashMove(dodgeDirection, m_DashDistance, m_DashTime);
        }
    }
}
