using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIEquipment : MonoBehaviour
{
    public UIDocument document;

    private VisualElement m_EquipmentRoot;
    private VisualElement m_EquipmentContainer;

    private ListView m_EquipmentList;
    //private Label m_DescriptionLabel;

    public List<Item> m_EquipmentData;

    public bool EquipmentOpen { get; private set; }

    private void Start()
    {
        InitEquipmentUI();
    }

    public void InitEquipmentUI()
    {
        if (GroupController.Instance.Party == null || GroupController.Instance.Party.Count == 0)
        {
            return;
        }

        AllyController player = GroupController.Instance.ActiveCharacter;
        m_EquipmentData = new List<Item> {
        player.Head,
        player.Arms,
        player.Chest,
        player.Legs,
        player.Boots,
        player.RightHand};

        m_EquipmentRoot =
            document.rootVisualElement;

        m_EquipmentContainer =
            m_EquipmentRoot.Q<VisualElement>("EquipmentContainer");

        m_EquipmentList =
            m_EquipmentRoot.Q<ListView>("EquipmentList");

        //m_DescriptionLabel =
        //  descriptionBox.Q<Label>("DescriptionLabel");

        m_EquipmentList.selectionChanged += OnSelectionChanged;

        RefreshEquipment();

        CloseEquipment();
    }

    public void RefreshEquipment()
    {
        AllyController player = GroupController.Instance.ActiveCharacter;
        m_EquipmentData = new List<Item> {
        player.Head,
        player.Arms,
        player.Chest,
        player.Legs,
        player.Boots,
        player.RightHand};

        if (m_EquipmentData == null)
            return;

        m_EquipmentList.itemsSource = m_EquipmentData;

        m_EquipmentList.makeItem = () =>
        {
            VisualElement row = new VisualElement();

            row.style.flexDirection = FlexDirection.Row;

            Label slotLabel = new Label();
            slotLabel.name = "SlotLabel";
            slotLabel.style.width = 200;

            Label itemLabel = new Label();
            itemLabel.name = "ItemLabel";
            itemLabel.style.width = 200;

            row.Add(slotLabel);
            row.Add(itemLabel);

            return row;
        };

        m_EquipmentList.bindItem = (element, index) =>
        {
            Label slotLabel =
                element.Q<Label>("SlotLabel");

            Label itemLabel =
                element.Q<Label>("ItemLabel");

            string[] slotNames =
            {
        "Head",
        "Arms",
        "Chest",
        "Legs",
        "Boots",
        "Right Hand"
    };

            slotLabel.text = slotNames[index];

            if (m_EquipmentData[index] != null)
            {
                itemLabel.text =
                    m_EquipmentData[index].itemName;
            }
            else
            {
                itemLabel.text = "Empty";
            }
        };

        m_EquipmentList.Rebuild();
    }

    void OnSelectionChanged(IEnumerable<object> items)
    {
        foreach (var item in items)
        {
            Item data = item as Item;
            /*
            if (data != null)
            {
                m_DescriptionLabel.text =
                    data.description;
            }
            */
            
            //DialogManager.Instance.EquipmentDialog();
        }
    }

    public void OpenEquipment()
    {
        m_EquipmentContainer.style.display =
            DisplayStyle.Flex;

        EquipmentOpen = true;

        RefreshEquipment();

        m_EquipmentList.ClearSelection();
    }

    public void CloseEquipment()
    {
        m_EquipmentContainer.style.display =
            DisplayStyle.None;

        EquipmentOpen = false;

        m_EquipmentList.ClearSelection();
    }
}