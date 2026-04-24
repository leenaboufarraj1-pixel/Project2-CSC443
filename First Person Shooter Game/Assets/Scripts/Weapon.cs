using UnityEngine;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Animator bowAnimator;

    [Header("Weapon Data")]
    [SerializeField] private WeaponData weaponData;
    public int currentAmmo;
    [SerializeField] public int maxAmmo = 30; // Default to something above 0

    [Header("Ammo UI Polish")]
    [SerializeField] private GameObject outOfAmmoUI; // Drag your UI Text/Object here in the Inspector

    public WeaponData Data => weaponData;
    public int CurrentAmmo => currentAmmo;
    public bool HasAmmo => currentAmmo > 0;

    void Start()
    {
        // Start full based on the data file
        currentAmmo = weaponData.maxAmmo;

        // Ensure the UI is hidden at start
        if (outOfAmmoUI != null) outOfAmmoUI.SetActive(false);
    }

    private void Awake()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    public void Shoot()
    {
        // 1. CHECK AMMO FIRST (Highest Priority)
        if (currentAmmo <= 0)
        {
            Debug.Log("UI SHOULD BE SHOWING NOW!"); // <--- Add this
            // Play Sound immediately
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.outOfAmmoSound);
            }

            // Show UI Warning Text
            if (outOfAmmoUI != null)
            {
                outOfAmmoUI.SetActive(true);
                CancelInvoke(nameof(HideAmmoWarning)); // Reset timer if player keeps clicking
                Invoke(nameof(HideAmmoWarning), 1.0f); // Hide after 1 second
            }
            return;
        }

        // 2. Safety check for the EventSystem (UI Blocking)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 3. Shop Safety check
        if (ShopManager.Instance != null && ShopManager.Instance.isShopOpen)
        {
            return;
        }

        // --- ACTUAL SHOOTING LOGIC ---
        currentAmmo--;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(AudioManager.Instance.shootSound);

        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger("ShootArrow");
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green, 0.5f);

            EnemyHealth health = hit.collider.GetComponentInParent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage((int)weaponData.damage);
                Debug.Log("Hit " + hit.collider.name + "!");
            }
        }
    }

    private void HideAmmoWarning()
    {
        if (outOfAmmoUI != null)
            outOfAmmoUI.SetActive(false);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, weaponData.maxAmmo);

        // Hide warning if we just bought ammo
        if (currentAmmo > 0 && outOfAmmoUI != null)
        {
            outOfAmmoUI.SetActive(false);
        }
    }
}