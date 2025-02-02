using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowSmooth : MonoBehaviour
{
    public Camera cam; // reference to main camera
    public float zoomOutFactor = 0.2f; // how much camera zooms out per growth
    public Transform player;
    public float smoothSpeed = 5f; // higher = snappier, lower = floaty
    public float zoomSpeed = 2f; // how fast zoom adjusts
    private float targetZoom; // store target zoom level

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main; // auto assign camera if not set
        }

        targetZoom = cam.orthographicSize; // initialize zoom
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // smoothly follow player
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // smoothly zoom in / out
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
        }
    }

    // call this from PlayerScript to update zoom
    public void AdjustZoom(float zoomAmount)
    {
        targetZoom += zoomAmount; // adjust target zoom
        targetZoom = Mathf.Clamp(targetZoom, 3f, 10f); // prevent extreme zoom
    }
}
