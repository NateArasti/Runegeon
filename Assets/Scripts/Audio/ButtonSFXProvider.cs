using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFXProvider : MonoBehaviour
{
    private Button m_Button;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    private void Start()
    {
        m_Button.onClick.AddListener(DefaultButtonClickSFX.Instance.PlaySFX);
    }

    public void ForcePlay() => DefaultButtonClickSFX.Instance.PlaySFX();
}
