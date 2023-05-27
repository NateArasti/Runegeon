using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class HeartsHealthBar : MonoBehaviour
{
    [SerializeField] private Image m_HeartPrefab;
    [SerializeField] private Sprite[] m_HeartStages;
    private List<Image> m_CurrentHearts = new();

    public void SetMaxHealth(int maxHealth)
    {
        var heartCount = maxHealth / 4;

        transform.DestroyChildren();

        for (int i = 0; i < heartCount; i++)
        {
            m_CurrentHearts.Add(Instantiate(m_HeartPrefab, transform));
        }
    }

    public void SetCurrentHealth(int currentHealth)
    {
        var fullHeartCount = currentHealth / 4;
        if (m_CurrentHearts.Count < fullHeartCount) return;
        for (int i = 0; i < fullHeartCount; i++)
        {
            m_CurrentHearts[i].sprite = m_HeartStages[4];
        }
        if(fullHeartCount < m_CurrentHearts.Count)
        {
            var leftHealth = currentHealth - fullHeartCount * 4;
            m_CurrentHearts[fullHeartCount].sprite = m_HeartStages[leftHealth];
            for (int i = fullHeartCount + 1; i < m_CurrentHearts.Count; i++)
            {
                m_CurrentHearts[i].sprite = m_HeartStages[0];
            }
        }
    }
}
