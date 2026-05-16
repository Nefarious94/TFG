using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameManager;

public class UIPause : MonoBehaviour
{
    public UIDocument document;

    private VisualElement m_PauseRoot;
    private VisualElement m_PauseContainer;
    private ListView m_PauseList;
    private List<string> m_PauseBase;
    private List<string> m_PauseDungeon;
    List<string> list;

    public bool pauseOpen;

    public bool PauseOpen { get; private set; }

    private void Start()
    {
        InitPauseUI();
    }

    void InitPauseUI()
    {
        m_PauseRoot = document.rootVisualElement;
        m_PauseContainer = m_PauseRoot.Q<VisualElement>("PauseContainer");
        m_PauseList = m_PauseRoot.Q<ListView>("PauseList");

        m_PauseBase = new List<string>()
        {
            "Inventory",
            "Equipment",
            "Abilities",
            "Options",
            "Save",
            "Load",
            "Main Menu",
            "Exit"
        };

        m_PauseDungeon = new List<string>()
        {
            "Inventory",
            "Equipment",
            "Abilities",
            "Options",
            "Load",
            "Main Menu",
            "Exit"
        };

        m_PauseList.selectionChanged += OnSelectionChanged;

        RefreshPause();

        ClosePause();
    }

    void RefreshPause()
    {
        
        if (GameManager.Instance.CurrentMode == GameMode.Base)
        {
            list = m_PauseBase;
        }
        else
        {
            list = m_PauseDungeon;
        }

        m_PauseList.itemsSource = list;

        m_PauseList.makeItem = () =>
            {
                Label label = new Label();
                //label.style.unityTextAlign = TextAnchor.MiddleLeft;
                //label.style.paddingLeft = 12;
                return label;
            };

        m_PauseList.bindItem = (element, index) =>
        {
            Label label = element as Label;
            label.text = list[index];
        };

        m_PauseList.Rebuild();
    }

    void OnSelectionChanged(IEnumerable<object> items)
    {
        foreach (var item in items)
        {
            string option = item as string;

            switch (option)
            {
                case "Inventory":
                    UIManager.Instance.OpenInventory();
                    break;

                case "Equipment":

                    Debug.Log("OPEN EQUIPMENT");

                    break;

                case "Abilities":

                    UIManager.Instance.OpenAbility();
                    break;

                case "Options":

                    Debug.Log("OPEN OPTIONS");

                    break;

                case "Save":

                    Debug.Log("SAVE GAME");

                    break;

                case "Load":

                    Debug.Log("LOAD GAME");

                    break;

                case "Main Menu":

                    Debug.Log("RETURN MAIN MENU");

                    break;

                case "Exit":
                    Application.Quit();
                    break;
            }
        }
        m_PauseList.ClearSelection();
    }

    public void OpenPause()
    {
        m_PauseContainer.style.display = DisplayStyle.Flex;
        pauseOpen = true;
        m_PauseList.ClearSelection();
        m_PauseContainer.style.height = m_PauseList.fixedItemHeight * list.Count + 5f * list.Count;
    }

    public void ClosePause()
    {
        m_PauseContainer.style.display = DisplayStyle.None;
        pauseOpen = false;
        m_PauseList.ClearSelection();
    }
}