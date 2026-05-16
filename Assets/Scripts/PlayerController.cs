using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GroupController m_Group;

    public float MoveDelay = 0.15f;

    public void Init(GroupController group)
    {
        m_Group = group;
    }

    private void Update()
    {
        if (UIManager.Instance.pause.pauseOpen)
            return;

        Vector2Int direction = Vector2Int.zero;

        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            direction = Vector2Int.up;
        else if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            direction = Vector2Int.down;
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
            direction = Vector2Int.left;
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            direction = Vector2Int.right;

        if (direction != Vector2Int.zero)
        {
            m_Group.groupMove(direction);
            //m_Timer = MoveDelay;
        }

        // cambiar modo
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            m_Group.groupMode = !m_Group.groupMode;
        }
    }
}