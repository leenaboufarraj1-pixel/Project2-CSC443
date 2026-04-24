using UnityEngine;
using StarterAssets;
using System.Collections;

public class ActiveWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject initialWeaponObject;

    private Weapon currentWeapon;
    private StarterAssetsInputs inputs;
    private float nextFireTime = 0f;
    private const string SHOOT_ANIMATION_TRIGGER = "ShootArrow";

    [Header("Juice")]
    [SerializeField] private float shakeDuration = 0.08f;
    [SerializeField] private float shakeIntensity = 0.04f;

    // New variables to stop the 'Update' conflict
    private Vector3 weaponBasePosition = new Vector3(0.35f, -0.4f, 0.6f);
    private Vector3 shakeOffset = Vector3.zero;

    // Make sure it looks exactly like this
    public Weapon CurrentWeapon => currentWeapon;

    private void Awake()
    {
        inputs = GetComponentInParent<StarterAssetsInputs>();
    }

    private void Start()
    {
        WeaponSwitcher switcher = GetComponent<WeaponSwitcher>();
        if (switcher != null && initialWeaponObject != null)
        {
            switcher.UnlockWeapon(initialWeaponObject.GetComponent<Weapon>());
        }
        else if (initialWeaponObject != null)
        {
            UpgradeToWeapon(initialWeaponObject);
        }
    }

    private void Update()
    {
        if (inputs.shoot) Debug.Log("Clicking Shoot Input!");
        // FIX: Instead of a hardcoded Vector3, we add the shakeOffset
        transform.localPosition = weaponBasePosition + shakeOffset;
        transform.localRotation = Quaternion.identity;

        HandleShoot();
    }

    private void HandleShoot()
    {
        if (currentWeapon == null || Time.time < nextFireTime) return;

        if (inputs.shoot)
        {
            if (currentWeapon.Data != null && !currentWeapon.Data.isAutomatic)
            {
                inputs.ShootInput(false);
            }
            ExecuteShot();
        }
    }

    private void ExecuteShot()
    {
        if (currentWeapon == null) return;

        nextFireTime = Time.time + (1.0f / currentWeapon.Data.fireRate);

        if (animator != null)
        {
            animator.SetTrigger(SHOOT_ANIMATION_TRIGGER);
        }

        // Start shaking the offset, not the transform directly
        StartCoroutine(ShakeWeaponOffset());

        currentWeapon.Shoot();
    }

    private IEnumerator ShakeWeaponOffset()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            // We update the offset, which the Update() loop then applies
            shakeOffset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset offset to zero when done
        shakeOffset = Vector3.zero;
    }

    public void SwitchWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        Animator newAnim = newWeapon.GetComponentInChildren<Animator>();
        if (newAnim != null) animator = newAnim;
    }

    public void UpgradeToWeapon(GameObject newWeaponObject)
    {
        // 1. Find the parent of the new weapon (which is the WeaponSwitcher object)
        Transform weaponContainer = newWeaponObject.transform.parent;

        // 2. Hide EVERY bow inside that container so they don't overlap
        if (weaponContainer != null)
        {
            foreach (Transform child in weaponContainer)
            {
                child.gameObject.SetActive(false);
            }
        }

        // 3. Show ONLY the one we just bought
        newWeaponObject.SetActive(true);

        // 4. Update references so shooting still works
        Weapon weaponScript = newWeaponObject.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            currentWeapon = weaponScript;

            // Grab the specific animator for this bow
            animator = newWeaponObject.GetComponentInChildren<Animator>();
        }
    }
}