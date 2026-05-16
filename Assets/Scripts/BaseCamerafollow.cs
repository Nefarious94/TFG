using UnityEngine;

public class BaseCamerafollow : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }
}
