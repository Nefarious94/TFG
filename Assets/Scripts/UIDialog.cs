using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDialog : MonoBehaviour
{
    public UIDocument document;

    private VisualElement m_DialogRoot;
    private VisualElement m_DialogContainer;
    private VisualElement m_DialogOptions;
    private Label m_DialogLabel;
    private ListView m_DialogList;

    private List<string> m_CurrentOptions;
    private Action<int> m_OnOptionSelected;


    public bool DialogOpen { get; private set; }

    private void Start()
    {
        InitDialogUI();
    }

    void InitDialogUI()
    {
        m_DialogRoot = document.rootVisualElement;

        m_DialogContainer = m_DialogRoot.Q<VisualElement>("DialogContainer");

        m_DialogOptions = m_DialogRoot.Q<VisualElement>("DialogOptions");

        m_DialogLabel = m_DialogContainer.Q<Label>("DialogLabel");

        m_DialogList = m_DialogOptions.Q<ListView>("DialogList");

        m_DialogList.itemsChosen += OnItemsChosen;

        m_DialogList.selectionType = SelectionType.Single;
        m_DialogList.focusable = false;

        CloseDialog();
    }

    public void OpenDialog(string text, List<string> options, Action<int> callback)
    {

        m_DialogContainer.style.display = DisplayStyle.Flex;

        m_DialogOptions.style.display = DisplayStyle.Flex;

        DialogOpen = true;

        m_DialogLabel.text = text;

        m_CurrentOptions = options;

        m_OnOptionSelected = callback;

        m_DialogOptions.style.height = m_DialogList.fixedItemHeight * m_CurrentOptions.Count + 5f * m_CurrentOptions.Count;

        RefreshDialog();

        m_DialogList.ClearSelection();
    }

    public void CloseDialog()
    {
        m_DialogContainer.style.display = DisplayStyle.None;

        m_DialogOptions.style.display = DisplayStyle.None;

        DialogOpen = false;

        m_DialogList.ClearSelection();
    }

    void RefreshDialog()
    {
        m_DialogList.itemsSource = m_CurrentOptions;

        m_DialogList.makeItem = () =>
        {
            Label label = new Label();

            label.style.unityTextAlign =
                TextAnchor.MiddleLeft;

            return label;
        };

        m_DialogList.bindItem = (element, index) =>
            {
                Label label = element as Label;

                label.text =
                    m_CurrentOptions[index];
            };

        m_DialogList.Rebuild();
    }

    void OnSelectionChanged(IEnumerable<object> items)
    {
        foreach (var item in items)
        {
            string option = item as string;

            int index = m_CurrentOptions.IndexOf(option);

            m_OnOptionSelected?.Invoke(index);
        }

        CloseDialog();
    }

    void OnItemsChosen(IEnumerable<object> items)
    {
        foreach (var item in items)
        {
            string option = item as string;

            int index = m_CurrentOptions.IndexOf(option);

            m_OnOptionSelected?.Invoke(index);
        }

        CloseDialog();
    }
}
