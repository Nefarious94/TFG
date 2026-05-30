using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public UIPause pause;
    public UIInventory inventory;
    public UIAbility ability;
    public UIHealthBar healthBar;
    public UIManaBar manaBar;
    public UIDialog dialog;
    public UIFloor floor;
    public UIGold gold;
    public UIEquipment equipment;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindUI();
    }

    private void Update()
    {
        HandlePause();
        if (GameManager.Instance.CurrentMode == GameManager.GameMode.Dungeon)
        {
            healthBar.UpdateHealthBar(GameManager.Instance.GroupController.ActiveCharacter.currentHP, GameManager.Instance.GroupController.ActiveCharacter.maxHP);
            manaBar.UpdateManaBar(GameManager.Instance.GroupController.ActiveCharacter.currentMana, GameManager.Instance.GroupController.ActiveCharacter.maxMana);
            floor.UpdateFloor(GameManager.Instance.dungeonFloor);
        }
    }

    void HandlePause()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (inventory.InventoryOpen)
            {
                inventory.CloseInventory();
                pause.OpenPause();
                return;
            }

            if (ability.AbilityOpen)
            {
                ability.CloseAbility();
                pause.OpenPause();
                return;
            }

            if (equipment.EquipmentOpen)
            {
                equipment.CloseEquipment();
                pause.OpenPause();
                return;
            }

            if (pause.pauseOpen)
            {
                pause.ClosePause();
                return;
            }

            pause.OpenPause();
        }
    }

    public void OpenInventory()
    {
        pause.ClosePause();
        inventory.OpenInventory();
    }

    public void OpenAbility()
    {
        pause.ClosePause();
        ability.OpenAbility();
    }

    public void OpenEquipment()
    {
        pause.ClosePause();
        equipment.OpenEquipment();
    }

    public bool BlockMove()
    {
        if (pause.pauseOpen || pause.PauseOpen || inventory.InventoryOpen || ability.AbilityOpen || dialog.DialogOpen || equipment.EquipmentOpen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUI();
    }

    void FindUI()
    {
        pause = FindAnyObjectByType<UIPause>();
        inventory = FindAnyObjectByType<UIInventory>();
        ability = FindAnyObjectByType<UIAbility>();
        healthBar = FindAnyObjectByType<UIHealthBar>();
        manaBar = FindAnyObjectByType<UIManaBar>();
        dialog = FindAnyObjectByType<UIDialog>();
        floor = FindAnyObjectByType<UIFloor>();
        gold = FindAnyObjectByType<UIGold>();
        equipment = FindAnyObjectByType<UIEquipment>();
    }
}