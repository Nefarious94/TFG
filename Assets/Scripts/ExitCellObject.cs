using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile EndTile;
    private bool m_PlayerInside;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        GameManager.Instance.BoardManager.SetCellTile(coord, EndTile, true);
    }

    public override void PlayerEntered()
    {
        if (m_PlayerInside)
            return;

        m_PlayerInside = true;

        DialogManager.Instance.ExitCellDialog();
    }

    public ExitCellObject ResetExit()
    {
        m_PlayerInside = false;
        return this;
    }
}
