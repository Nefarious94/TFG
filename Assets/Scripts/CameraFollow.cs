using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public float Speed = 5f;

    private void LateUpdate()
    {
        if (Target == null) return;

        Vector3 targetPos = new Vector3(
            Target.position.x,
            Target.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Speed * Time.deltaTime
        );
    }
}