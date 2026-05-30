using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAbility : MonoBehaviour
{
    public UIDocument document;

    private VisualElement m_AbilityRoot;
    private VisualElement m_AbilityContainer;

    private ListView m_AbilityList;
    private Label m_DescriptionLabel;

    public List<Ability> m_AbilityData;

    public bool AbilityOpen { get; private set; }

    private void Start()
    {
        InitAbilityUI();
    }

    public void InitAbilityUI()
    {
        if (GroupController.Instance.Party == null || GroupController.Instance.Party.Count == 0)
        {
            return;
        }

        m_AbilityData = GameManager.Instance.abilities;

        m_AbilityRoot =
            document.rootVisualElement;

        m_AbilityContainer =
            m_AbilityRoot.Q<VisualElement>("AbilityContainer");

        m_AbilityList =
            m_AbilityRoot.Q<ListView>("AbilityList");

        VisualElement descriptionBox =
            m_AbilityRoot.Q<VisualElement>("DescriptionBox");

        m_DescriptionLabel =
            descriptionBox.Q<Label>("DescriptionLabel");

        m_AbilityList.selectionChanged += OnSelectionChanged;

        RefreshAbility();

        CloseAbility();
    }

    public void RefreshAbility()
    {
        m_AbilityData = GroupController.Instance.ActiveCharacter.abilitiesUnlocked;

        if (m_AbilityData == null)
            return;

        m_AbilityList.itemsSource = m_AbilityData;

        m_AbilityList.makeItem = () =>
        {
            Label label = new Label();

            label.style.unityTextAlign =
                TextAnchor.MiddleLeft;

            return label;
        };

        m_AbilityList.bindItem = (element, index) =>
        {
            Label label = element as Label;

            label.text = m_AbilityData[index].abilityName;
        };

        m_AbilityList.Rebuild();
    }

    void OnSelectionChanged(IEnumerable<object> abilities)
    {
        foreach (var ability in abilities)
        {
            Ability data = ability as Ability;

            if (data != null)
            {
                m_DescriptionLabel.text =
                    data.description;
            }
            if (GameManager.Instance.CurrentMode == GameManager.GameMode.Dungeon)
            {
                DialogManager.Instance.AbilityDialog(data);
            }
        }
    }

    public void OpenAbility()
    {
        m_AbilityContainer.style.display =
            DisplayStyle.Flex;

        AbilityOpen = true;

        RefreshAbility();

        m_AbilityList.ClearSelection();
    }

    public void CloseAbility()
    {
        m_AbilityContainer.style.display =
            DisplayStyle.None;

        AbilityOpen = false;

        m_AbilityList.ClearSelection();
    }
}