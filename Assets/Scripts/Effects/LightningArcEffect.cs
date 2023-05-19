using UnityEngine;
using UnityExtensions;

public class LightningArcEffect : MonoBehaviour
{
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

    public void SetEffect(Vector3 startPosition, Vector3 endPosition, 
        float destroyDelay = 1, float collapseDelay = -1)
    {
        var direction = endPosition - startPosition;
        transform.position = startPosition + 0.5f * direction;
        transform.right = direction;
        transform.localScale = new Vector3(direction.magnitude, transform.localScale.y, transform.localScale.z);

        m_Lifetime = destroyDelay;
        if(collapseDelay > 0)
        {
            this.InvokeSecondsDelayed(() =>
            {
                transform.position = endPosition;
                transform.localScale = Vector3.right * 0.1f;
            }, collapseDelay);
        }
    }
}
