using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopManager.ToggleShop(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopManager.ToggleShop(false);
        }
    }
}