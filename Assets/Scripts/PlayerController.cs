using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GroupController m_Group;

    private bool m_DiagonalReleased = true;

    public void Init(GroupController group)
    {
        m_Group = group;
    }

    private void Update()
    {
        if (UIManager.Instance.BlockMove())
            return;

        Vector2Int direction = Vector2Int.zero;

        bool shift = Keyboard.current.leftShiftKey.isPressed;

        // --------------------------
        // MOVIMIENTO DIAGONAL
        // --------------------------

        if (shift)
        {
            bool up =
                Keyboard.current.wKey.isPressed ||
                Keyboard.current.upArrowKey.isPressed;

            bool down =
                Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed;

            bool left =
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed;

            bool right =
                Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed;

            if (up && left)
                direction = new Vector2Int(-1, 1);

            else if (up && right)
                direction = new Vector2Int(1, 1);

            else if (down && left)
                direction = new Vector2Int(-1, -1);

            else if (down && right)
                direction = new Vector2Int(1, -1);

            // reset input
            if (direction == Vector2Int.zero)
            {
                m_DiagonalReleased = true;
            }

            // mover solo una vez
            if (direction != Vector2Int.zero && m_DiagonalReleased)
            {
                m_DiagonalReleased = false;

                m_Group.groupMove(direction);
            }
        }

        // --------------------------
        // MOVIMIENTO NORMAL
        // --------------------------

        else
        {
            if (Keyboard.current.wKey.wasPressedThisFrame ||
                Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                direction = Vector2Int.up;
            }
            else if (Keyboard.current.sKey.wasPressedThisFrame ||
                     Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                direction = Vector2Int.down;
            }
            else if (Keyboard.current.aKey.wasPressedThisFrame ||
                     Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                direction = Vector2Int.left;
            }
            else if (Keyboard.current.dKey.wasPressedThisFrame ||
                     Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                direction = Vector2Int.right;
            }

            if (direction != Vector2Int.zero)
            {
                m_Group.groupMove(direction);
            }
        }

        // esperar turno
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            GameManager.Instance.TurnManager.EndAllyTurn();
            if (GroupController.Instance.groupMode)
            {
                GroupController.Instance.NextCharacter();
            }
        }

        // cambiar modo
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            m_Group.groupMode = !m_Group.groupMode;
        }
    }
}