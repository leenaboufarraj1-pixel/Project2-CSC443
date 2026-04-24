using UnityEngine;

public class XPPoint : MonoBehaviour
{
    private int xpValue = 100;

    [Header("Movement")]
    [SerializeField] float rotationSpeed = 100f; // Increased for better feel
    [SerializeField] float bobSpeed = 2f;       // How fast it moves up/down
    [SerializeField] float bobAmount = 0.2f;    // How far it moves up/down

    private Vector3 startPos;

    private void Start()
    {
        // 1. Move it up immediately so it's not in the floor
        transform.position += Vector3.up * 7f;

        // 2. Remember this height as the center for the bobbing
        startPos = transform.position;
    }

    private void Update()
    {
        // 1. Calculate the new Y rotation based on time
        // We store this in a variable so we can keep the X and Z fixed
        float rotationY = Time.time * rotationSpeed;

        // 2. Set the rotation directly: 90 on X, our Spinning value on Y, 0 on Z
        transform.rotation = Quaternion.Euler(90f, rotationY, 0f);

        // 3. Floating up and down (Keep your existing bobbing logic)
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    public void Init(int value)
    {
        xpValue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(xpValue); // Uses the value passed from the enemy
            }


            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.pickupSound);
            }

            Destroy(gameObject);
        }
    }
}