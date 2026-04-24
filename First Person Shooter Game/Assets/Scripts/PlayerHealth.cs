using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Required for IEnumerator/Coroutines

public class PlayerHealth : MonoBehaviour
{

    [Header("Damage Feedback")]
    [SerializeField] private Image damageImage;
    [SerializeField] private float flashSpeed = 5f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.4f);

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthBarFill; // Drag the "Fill" image of the slider here


    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
        UpdateUI();
    }

    void Update()
    {
        // Ensure damageImage isn't null and has some alpha left to fade
        if (damageImage != null && damageImage.color.a > 0)
        {
            // Smoothly fade the color toward clear (alpha 0)
            Color targetColor = Color.clear;
            damageImage.color = Color.Lerp(damageImage.color, targetColor, flashSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        // NEW: Check if the shop is open before processing damage
        if (ShopManager.Instance != null && ShopManager.Instance.isShopOpen)
        {
            return; // Exit the function early; no damage taken
        }

        currentHealth -= damage;

        if (damageImage != null) damageImage.color = flashColor;

        StartCoroutine(ShakeCamera());
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Ensure this is PUBLIC so the ShopManager can see it
    public void Heal(int amount)
    {
        currentHealth += amount;

        // Clamp the health so it never exceeds maxHealth
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // CRITICAL: You must call UpdateUI() here or the bar won't move!
        UpdateUI();

        Debug.Log("Healed! Current Health: " + currentHealth);
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;

        // POLISH: Dynamic Color Change
        if (healthBarFill != null)
        {
            if (currentHealth >= 50f)
                healthBarFill.color = Color.green;
            else if (currentHealth >= 20f)
                healthBarFill.color = Color.yellow;
            else
                healthBarFill.color = Color.red;
        }
    }

    void Die()
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ShowGameOver();
        }
        else
        {
            Debug.LogError("MenuManager is missing from the scene!");
        }
    }

    private IEnumerator ShakeCamera()
    {
        // Using main camera's parent or local space to avoid jumping issues
        Transform camTrans = Camera.main.transform;
        Vector3 originalPos = camTrans.localPosition;
        float elapsed = 0f;

        while (elapsed < 0.15f)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            camTrans.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        camTrans.localPosition = originalPos;
    }
}