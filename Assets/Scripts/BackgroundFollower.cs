using UnityEngine;

public class BackgroundFollower : MonoBehaviour
{
    public Transform cameraTransform;  // Reference to the camera
    public float zOffset = 10f;        // How far behind other objects (should be positive)

    void Start()
    {
        // If camera reference not set, find the main camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Ensure background is behind other objects
        Vector3 pos = transform.position;
        pos.z = zOffset;
        transform.position = pos;
    }

    void LateUpdate()
    {
        // Update position to match camera's x and y, keeping our z offset
        Vector3 newPos = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            zOffset
        );
        transform.position = newPos;
    }
}