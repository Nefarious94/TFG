using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIInventory : MonoBehaviour
{
    public UIDocument inventoryDocument;

    private VisualElement m_InventoryRoot;
    private VisualElement m_InventoryContainer;
    private VisualElement m_DescriptionBox;

    private ListView m_ItemList;
    private Label m_DescriptionLabel;

    private Inventory m_InventoryData;

    public bool InventoryOpen { get; private set; }

    private void Start()
    {
        InitInventoryUI();
    }

    void InitInventoryUI()
    {
        m_InventoryData = GameManager.Instance.inventoryPlayer;
        m_InventoryRoot = inventoryDocument.rootVisualElement;
        m_InventoryContainer = m_InventoryRoot.Q<VisualElement>("InventoryContainer");
        m_ItemList = m_InventoryRoot.Q<ListView>("ItemList");
        m_DescriptionBox = m_InventoryRoot.Q<VisualElement>("DescriptionBox");
        m_DescriptionLabel = m_DescriptionBox.Q<Label>("DescriptionLabel");
        m_ItemList.selectionChanged += OnSelectionChanged;

        RefreshInventory();
        CloseInventory();
    }

    public void RefreshInventory()
    {
        if (m_InventoryData == null)
            return;

        m_ItemList.itemsSource = m_InventoryData.ItemsList;

        m_ItemList.makeItem = () =>
        {
            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        };

        m_ItemList.bindItem = (element, index) =>
        {
            Label label = element as Label;
            label.text = m_InventoryData.ItemsList[index].itemName;
        };
        m_ItemList.Rebuild();
    }

    void OnSelectionChanged(IEnumerable<object> items)
    {
        foreach (var item in items)
        {
            Item data = item as Item;
            if (data != null)
            {
                m_DescriptionLabel.text = data.description;
            }
            DialogManager.Instance.InventoryDialog(data.itemID);
        }
    }

    public void OpenInventory()
    {
        m_InventoryContainer.style.display = DisplayStyle.Flex;
        InventoryOpen = true;
        RefreshInventory();
        m_ItemList.ClearSelection();
    }

    public void CloseInventory()
    {
        m_InventoryContainer.style.display = DisplayStyle.None;
        InventoryOpen = false;
        m_ItemList.ClearSelection();
    }
}