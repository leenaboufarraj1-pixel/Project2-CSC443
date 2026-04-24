using UnityEngine;
using StarterAssets;
using UnityEngine.UI;
using TMPro; // Added for the status text

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI statusText; // Assign this in Inspector

    [Header("Player References")]
    [SerializeField] private ActiveWeapon activeWeapon;
    [SerializeField] private WeaponSwitcher weaponSwitcher;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Prices")]
    [SerializeField] private int ammoPrice = 300;
    [SerializeField] private int healthPrice = 400;
    [SerializeField] private int longBowPrice = 500;
    [SerializeField] private int CrossBowPrice = 600;

    [Header("Weapon References")]
    [SerializeField] private Weapon longBow;
    [SerializeField] private Weapon crossBow;

    public bool isShopOpen = false;
    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (statusText != null) statusText.text = "";
    }

    public void ToggleShop(bool isOpen)
    {
        isShopOpen = isOpen;
        shopPanel.SetActive(isOpen);

        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;

        var controller = activeWeapon.GetComponentInParent<FirstPersonController>();
        if (controller != null) controller.enabled = !isOpen;

        // Clear status every time shop opens
        if (statusText != null) statusText.text = "";
    }

    public void BuyAmmo()
    {
        if (ScoreManager.Instance.GetCurrentScore() >= ammoPrice)
        {
            Weapon currentWeapon = weaponSwitcher.GetActiveWeapon();
            if (currentWeapon != null)
            {
                if (ScoreManager.Instance.TrySpendScore(ammoPrice))
                {
                    currentWeapon.currentAmmo = currentWeapon.Data.maxAmmo;
                    ShowStatus("Ammo Refilled!", Color.green);
                    PlaySuccessSound();
                }
            }
        }
        else ShowStatus("Not Enough Points!", Color.red);
    }

    public void BuyHealth()
    {
        if (ScoreManager.Instance.GetCurrentScore() >= healthPrice)
        {

            if (ScoreManager.Instance.TrySpendScore(healthPrice))
            {
         
                playerHealth.Heal(25);

                ShowStatus("Health Restored!", Color.green);
                PlaySuccessSound();
            }
        }
        else
        {
            ShowStatus("Not Enough Points!", Color.red);
        }
    }

    public void BuyLongBow()
    {
        if (ScoreManager.Instance.GetCurrentScore() >= longBowPrice)
        {
            if (ScoreManager.Instance.TrySpendScore(longBowPrice))
            {
                weaponSwitcher.UnlockWeapon(longBow);
                ShowStatus("Long Bow Unlocked!", Color.gold);
                PlaySuccessSound();
            }
        }
        else ShowStatus("Not Enough Points!", Color.red);
    }

    public void BuyCrossBow()
    {
        if (ScoreManager.Instance.GetCurrentScore() >= CrossBowPrice)
        {
            if (ScoreManager.Instance.TrySpendScore(CrossBowPrice))
            {
                weaponSwitcher.UnlockWeapon(crossBow);
                ShowStatus("Crossbow Unlocked!", Color.gold);
                PlaySuccessSound();
            }
        }
        else ShowStatus("Not Enough Points!", Color.red);
    }

    private void ShowStatus(string message, Color color)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.color = color;

        CancelInvoke(nameof(ClearStatus));
        Invoke(nameof(ClearStatus), 2f);
    }

    private void ClearStatus() => statusText.text = "";

    private void PlaySuccessSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(AudioManager.Instance.purchaseSuccessSound);
    }

    public void CloseShop() => ToggleShop(false);
}