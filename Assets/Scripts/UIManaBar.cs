using UnityEngine;
using UnityEngine.UIElements;

public class UIManaBar : MonoBehaviour
{
    public UIDocument document;
    public VisualElement m_ManaBarRoot;
    public VisualElement m_ManaBar;
    public VisualElement m_ManaBackground;
    public Label m_ManaPoints;

    private void Start()
    {
        InitManaBarUI();
    }

    void InitManaBarUI()
    {
        m_ManaBarRoot = document.rootVisualElement;
        m_ManaBar = m_ManaBarRoot.Q<VisualElement>("ManaBar");
        m_ManaBackground = m_ManaBar.Q<VisualElement>("ManaBackground");
        m_ManaPoints = m_ManaBarRoot.Q<Label>("ManaPoints");
    }

    public void UpdateManaBar(int currentMana, int maxMana)
    {
        float percent = ((float)currentMana / (float)maxMana) * 100f;
        m_ManaBackground.style.width = Length.Percent(percent);
        m_ManaPoints.text = currentMana + "/" + maxMana;
    }
}
