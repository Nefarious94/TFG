using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasePlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public float speed = 3.0f;
    Vector2 moveDirection = new Vector2(1, 0);

    public Vector2 dungeonEntrance = new Vector2(-1.5f, 23.25f);
    public float triggerRadius = 0.5f;
    private bool DungeonDialog;

    void Awake()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (UIManager.Instance.pause.pauseOpen)
            return;
        if (UIManager.Instance.dialog.DialogOpen)
            return;

        move = MoveAction.ReadValue<Vector2>();


        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        CheckDungeonEntrance();
    }

    void FixedUpdate()
    {
        if (UIManager.Instance.pause.pauseOpen)
            return;
        if (UIManager.Instance.dialog.DialogOpen)
            return;

        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void CheckDungeonEntrance()
    {
        float distance = Vector2.Distance(transform.position, dungeonEntrance);

        if (distance < triggerRadius)
        {
            if (!DungeonDialog)
            {
                DungeonDialog = true;

                DialogManager.Instance.DungeonEntranceDialog();
            }
        }
        else
        {
            DungeonDialog = false;
        }
    }
}
