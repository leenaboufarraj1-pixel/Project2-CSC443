using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Weapon Sounds")]
    public AudioClip shootSound;
    public AudioClip outOfAmmoSound;

    [Header("Enemy Sounds")]
    public AudioClip enemyDeathSound;

    [Header("Game Sounds")]
    public AudioClip pickupSound;
    public AudioClip uiClickSound;

    [Header("Shop Sounds")]
    public AudioClip purchaseSuccessSound;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource source = GetComponent<AudioSource>();
            source.PlayOneShot(clip);
        }
    }
}