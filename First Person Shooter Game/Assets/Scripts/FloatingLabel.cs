using UnityEngine;

public class FloatingLabel : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.2f; // How high it moves
    [SerializeField] private float floatFrequency = 1.0f; // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        // Store the position you set in the Inspector as the "anchor"
        startPos = transform.localPosition;
    }

    void Update()
    {
        // 1. ALWAYS FACE PLAYER (Billboard logic)
        // We look at the player but keep the label's rotation aligned with the camera
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);

        // 2. FLOAT UP AND DOWN
        // Uses a Sine wave to create a smooth, oscillating movement
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0, newY, 0);
    }
}