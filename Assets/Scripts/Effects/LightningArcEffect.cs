using UnityEngine;
using UnityExtensions;

public class LightningArcEffect : MonoBehaviour
{
    [SerializeField] private Transform m_StartPosition;
    [SerializeField] private Transform m_EndPosition;
    [SerializeField] private float m_Lifetime = 1;

    private void Update()
    {
        if(m_Lifetime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        m_Lifetime -= Time.deltaTime;
    }

    public void SetEffect(Vector3 startPosition, Vector3 endPosition, float destroyDelay = 1, float collapseDelay = -1)
    {
        transform.position = startPosition;

        m_StartPosition.position = startPosition;
        m_EndPosition.position = endPosition;

        m_Lifetime = destroyDelay;
        if(collapseDelay > 0)
        {
            this.InvokeSecondsDelayed(() =>
            {
                m_StartPosition.position = endPosition;
                m_EndPosition.position = endPosition;
            }, collapseDelay);
        }
    }
}
