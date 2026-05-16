using UnityEngine;
using UnityEngine.InputSystem;

public class BaseVendor : MonoBehaviour
{
    public GameManager gamemanager;
    public bool m_PlayerNearby;
    void Update()
    {
        if (!m_PlayerNearby)
        {
            return;
        }
        else 
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenShop();
            }
        }
        gamemanager = FindFirstObjectByType<GameManager>();
    }

    public void OpenShop()
    {
        DialogManager.Instance.ShopVendorDialog();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_PlayerNearby = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_PlayerNearby = false;
        }
    }
}
