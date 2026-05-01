using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float Speed = 5f;

    private Transform m_Target;

    private void LateUpdate()
    {
        var group = GameManager.Instance.BoardManager.GroupController;

        if (group == null || group.Party.Count == 0)
            return;

        m_Target = group.ActiveCharacter.transform;

        Vector3 targetPos = new Vector3(
            m_Target.position.x,
            m_Target.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Speed * Time.deltaTime
        );
    }
}