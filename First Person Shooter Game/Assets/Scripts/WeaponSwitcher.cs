using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] ActiveWeapon activeWeapon;
    StarterAssetsInputs inputs;
    List<Weapon> unlockedWeapons = new List<Weapon>();
    int currentIndex = -1;
    [SerializeField] private WeaponSwitcher weaponSwitcher;

    private void Awake()
    {
        inputs = GetComponentInParent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (inputs == null || !inputs.switchWeapon || unlockedWeapons.Count < 2) return;

        inputs.SwitchWeaponInput(false);
        int nextIndex = (currentIndex + 1) % unlockedWeapons.Count;
        EquipWeapon(nextIndex);
    }

    public void UnlockWeapon(Weapon weapon)
    {
        if (weapon == null || unlockedWeapons.Contains(weapon)) return;

        unlockedWeapons.Add(weapon);
        EquipWeapon(unlockedWeapons.Count - 1);
    }

    void EquipWeapon(int index)
    {
        for (int i = 0; i < unlockedWeapons.Count; i++)
        {
            // Force every weapon in the list to be inactive UNLESS it's the current index
            unlockedWeapons[i].gameObject.SetActive(i == index);
        }

        currentIndex = index;
        activeWeapon.SwitchWeapon(unlockedWeapons[currentIndex]);

        // EXTRA FIX: Tell the ActiveWeapon script to refresh its Animator reference
        // so the new weapon's animations actually play.
        activeWeapon.UpgradeToWeapon(unlockedWeapons[currentIndex].gameObject);
    }

    public Weapon GetActiveWeapon()
    {
        // Loops through children (your bows) and finds the one that is active
        foreach (Transform weapon in transform)
        {
            if (weapon.gameObject.activeSelf)
            {
                return weapon.GetComponent<Weapon>();
            }
        }
        return null;
    }
}