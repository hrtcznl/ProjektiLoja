using UnityEngine;

public class CameraFOVController : MonoBehaviour
{
    [Header("FOV Settings")]
    public Camera playerCamera;       // Drag your main camera here
    public float normalFOV = 60f;     // Default FOV
    public float zoomFOV = 30f;       // FOV when zooming
    public float smoothSpeed = 5f;    // How fast FOV changes

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        playerCamera.fieldOfView = normalFOV;
    }

    private void Update()
    {
        float targetFOV = normalFOV;

        // Check if right mouse button is held
        if (Input.GetMouseButton(1)) // 1 = right mouse button
        {
            targetFOV = zoomFOV;
        }

        // Smoothly interpolate FOV
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }
}