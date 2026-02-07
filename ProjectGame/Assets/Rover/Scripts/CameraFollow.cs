using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace VehicleBehaviour.Utils
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target (your car)")]
        [SerializeField] Transform target;

        [Header("Offset from car")]
        [SerializeField] Vector3 offset = new Vector3(0, 3, -6);

        [Header("Look above the car")]
        [SerializeField] float lookHeight = 5f; // how much higher the camera looks

        [Header("Smoothness")]
        [Range(0, 10)]
        [SerializeField] float lerpPositionMultiplier = 5f;

        [Range(0, 10)]
        [SerializeField] float lerpRotationMultiplier = 5f;

        [Header("Optional UI")]
        [SerializeField] Text speedometer;

        Rigidbody rb;
        WheelVehicle vehicle;

        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (target != null)
                vehicle = target.GetComponent<WheelVehicle>();
        }

        void FixedUpdate()
        {
            if (target == null) return;

            // Prevent physics weirdness
            rb.velocity = rb.velocity.normalized;

            Quaternion currentRot = transform.rotation;

            // Position behind the car
            Vector3 desiredPos = target.position + target.TransformDirection(offset);

            // Look slightly above the car
            Vector3 lookPos = target.position + new Vector3(0, lookHeight, 0);
            transform.LookAt(lookPos);

            // Smooth follow
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                Time.fixedDeltaTime * lerpPositionMultiplier
            );

            transform.rotation = Quaternion.Lerp(
                currentRot,
                transform.rotation,
                Time.fixedDeltaTime * lerpRotationMultiplier
            );

            // Keep above ground
            if (transform.position.y < 0.5f)
            {
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            }

            // Speedometer
            if (speedometer != null && vehicle != null)
            {
                speedometer.text = $"Speed: {(int)vehicle.Speed} Kph";
            }
        }
    }
}
