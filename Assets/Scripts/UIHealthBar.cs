using UnityEngine;
using UnityEngine.UIElements;

public class UIHealthBar : MonoBehaviour
{
    public UIDocument document;
    public VisualElement m_HealthBarRoot;
    public VisualElement m_HealthBar;
    public VisualElement m_HealthBackground;
    public Label m_HitPoints;

    private void Start()
    {
        InitHealthBarUI();
    }

    void InitHealthBarUI()
    {
        m_HealthBarRoot = document.rootVisualElement;
        m_HealthBar = m_HealthBarRoot.Q<VisualElement>("HealthBar");
        m_HealthBackground = m_HealthBar.Q<VisualElement>("HealthBackground");
        m_HitPoints = m_HealthBarRoot.Q<Label>("HitPoints");
    }

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        float percent = ((float)currentHP / (float)maxHP) * 100f;
        m_HealthBackground.style.width = Length.Percent(percent);
        m_HitPoints.text = currentHP + "/" + maxHP;
    }
}
