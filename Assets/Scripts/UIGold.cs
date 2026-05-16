using UnityEngine;
using UnityEngine.UIElements;

public class UIGold : MonoBehaviour
{
    public UIDocument document;
    private VisualElement m_GoldRoot;
    private VisualElement m_GoldContainer;
    private Label m_GoldLabel;

    private void Start()
    {
        InitGoldUI();
        UpdateGold(GameManager.Instance.m_Gold);
    }

    void InitGoldUI()
    {
        m_GoldRoot = document.rootVisualElement;
        m_GoldContainer = m_GoldRoot.Q<VisualElement>("GoldContainer");
        m_GoldLabel = m_GoldContainer.Q<Label>("GoldLabel");
    }

    public void UpdateGold(int gold)
    {
        m_GoldLabel.text = "Gold " + gold;
    }
}
