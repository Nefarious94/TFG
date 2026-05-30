using UnityEngine;

public class ChestObject : CellObject
{
    public int minGold = 5;
    public int maxGold = 25;
    public int rateMoney = 50;
    public int rateArmor = 25;
    public int rateWeapon = 25;

    public override void PlayerEntered()
    {
        var cell = GameManager.Instance.BoardManager.GetCellData(m_Cell);
        cell.ContainedObject = null;
        Destroy(gameObject);
        GameManager.Instance.OpenChest(this);
    }
}
