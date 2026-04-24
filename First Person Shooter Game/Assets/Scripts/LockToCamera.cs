using UnityEngine;

public class LockToCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;

    void LateUpdate() // LateUpdate runs AFTER the camera moves
    {
        if (cameraRoot != null)
        {
            transform.position = cameraRoot.position;
            transform.rotation = cameraRoot.rotation;
        }
    }
}