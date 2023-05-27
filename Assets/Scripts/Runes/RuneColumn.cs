using UnityEngine;

public class RuneColumn : MonoBehaviour
{
    [SerializeField] private BaseRuneEffect m_Rune;
    [Space]
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite m_EmptySprite;
    [SerializeField] private Sprite m_RuneSprite;
    private bool m_ContainsRune;

    private void Update()
    {
        if (RunesContainer.InStorage(m_Rune))
        {
            if (!m_ContainsRune)
            {
                m_ContainsRune = true;
                m_SpriteRenderer.sprite = m_RuneSprite;
            }
        }
        else
        {
            if (m_ContainsRune)
            {
                m_ContainsRune = false;
                m_SpriteRenderer.sprite = m_EmptySprite;
            }
        }
    }
}
