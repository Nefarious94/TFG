using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject m_PlayerInstance;

    public BaseCamerafollow BaseCamerafollow;
    public Vector2 spawnPosition = new Vector2(-4f, -4f);

    private void Awake()
    {
        Debug.Log("BaseManager Awake");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterBaseManager(this);
        }
    }

    public void Init()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (m_PlayerInstance == null)
        {
            m_PlayerInstance = Instantiate(playerPrefab);
        }

        m_PlayerInstance.transform.position = spawnPosition;

        Rigidbody2D rb = m_PlayerInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
        }

        if (BaseCamerafollow != null)
        {
            BaseCamerafollow.target = m_PlayerInstance.transform;
        }
    }
}