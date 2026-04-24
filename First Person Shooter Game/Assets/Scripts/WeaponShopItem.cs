using UnityEngine;
using StarterAssets;

public class WeaponShopItem : MonoBehaviour
{
    [Header("Weapon to Buy")]
    [SerializeField] private Weapon weaponPrefab; // Drag the weapon prefab/object here
    [SerializeField] private int cost = 100;

    [Header("Detection")]
    [SerializeField] private float interactRange = 3f;
    private Transform player;
  
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Basic check: is the player close enough?
        if (distance <= interactRange)
        {
            // You can show a UI prompt here like "Press E to Buy"
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPurchase();
            }
        }
    }

    private void TryPurchase()
    {
        // Replace 'PointManager' with whatever your point script is called
        ScoreManager points = player.GetComponent<ScoreManager>();

        if (points != null && points.currentScore >= cost)
        {
            points.TrySpendScore(cost);

            // Tell the switcher to unlock it!
            WeaponSwitcher switcher = player.GetComponentInChildren<WeaponSwitcher>();
            if (switcher != null)
            {
                switcher.UnlockWeapon(weaponPrefab);
                Debug.Log("Bought " + weaponPrefab.name);

                // Optional: Destroy the shop item so they can't buy it twice
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Not enough points!");
        }
    }
}