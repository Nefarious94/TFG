using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        GameManager.Instance.ChangeFood(AmountGranted);
    }
}
