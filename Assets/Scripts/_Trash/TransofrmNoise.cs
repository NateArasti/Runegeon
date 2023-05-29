using UnityEngine;

public class TransofrmNoise : MonoBehaviour
{
    [SerializeField] private float m_Scale = 1;
    private Vector3 m_StartPosition;

    private void Start()
    {
        m_StartPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = m_StartPosition + new Vector3(Random.value, Random.value) * m_Scale;
    }
}
