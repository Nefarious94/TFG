using UnityEngine;
using UnityEngine.UIElements;

public class UIFloor : MonoBehaviour
{
    public UIDocument document;
    private VisualElement m_FloorRoot;
    private VisualElement m_FloorContainer;
    private Label m_FloorLabel;

    private void Start()
    {
        InitFloorUI();
    }

    void InitFloorUI()
    {
        m_FloorRoot = document.rootVisualElement;
        m_FloorContainer = m_FloorRoot.Q<VisualElement>("FloorContainer");
        m_FloorLabel = m_FloorContainer.Q<Label>("FloorLabel");
    }

    public void UpdateFloor(int floor)
    {
        m_FloorLabel.text = "Floor " + floor;
    }
}
