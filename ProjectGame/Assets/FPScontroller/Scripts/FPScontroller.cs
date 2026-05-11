using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Camera & Look")]
    public Camera playerCamera;
    public float lookXLimit = 45f;

    [Header("Sensitivity Sliders")]
    public Slider sensitivitySlider1;
    public Slider sensitivitySlider2;

    public float minLookSpeed = 0.2f;
    public float maxLookSpeed = 10f;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public bool canMove = true;

    [Header("Audio")]
    public AudioSource walkingSound;
    public AudioListener cameraListener;
    public float audioDelay = 0.5f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    private float sensitivity01 = 1f;

    void Awake()
    {
        walkingSound.playOnAwake = false;
        walkingSound.Stop();

        if (cameraListener != null)
            cameraListener.enabled = false;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rotationX = playerCamera.transform.localEulerAngles.x;
        if (rotationX > 180f)
            rotationX -= 360f;

        StartCoroutine(EnableAudioAfterDelay());

        // Initialize from slider
        if (sensitivitySlider1 != null)
        {
            sensitivity01 = sensitivitySlider1.value / 100f;
        }

        // Bind events (IMPORTANT)
        if (sensitivitySlider1 != null)
            sensitivitySlider1.onValueChanged.AddListener(OnSensitivityChanged);

        if (sensitivitySlider2 != null)
            sensitivitySlider2.onValueChanged.AddListener(OnSensitivityChanged);

        SyncSliders();
    }

    IEnumerator EnableAudioAfterDelay()
    {
        yield return new WaitForSeconds(audioDelay);
        if (cameraListener != null)
            cameraListener.enabled = true;
    }

    void OnSensitivityChanged(float _)
    {
        float v = (sensitivitySlider1.value + sensitivitySlider2.value) * 0.5f;
        sensitivity01 = v / 100f;
        SyncSliders();
    }

    void SyncSliders()
    {
        float v = sensitivity01 * 100f;

        if (sensitivitySlider1.value != v)
            sensitivitySlider1.value = v;

        if (sensitivitySlider2.value != v)
            sensitivitySlider2.value = v;
    }

    void Update()
    {
        #region Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        float curSpeedX = canMove ? speed * v : 0;
        float curSpeedY = canMove ? speed * h : 0;

        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        bool moving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;

        if (moving && !Input.GetButton("Jump"))
        {
            if (!walkingSound.isPlaying)
                walkingSound.Play();
        }
        else
        {
            if (walkingSound.isPlaying)
                walkingSound.Stop();
        }
        #endregion

        #region Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            float t = Mathf.Pow(sensitivity01, 1.8f);
            float currentLookSpeed = Mathf.Lerp(minLookSpeed, maxLookSpeed, t);

            rotationX += -Input.GetAxis("Mouse Y") * currentLookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * currentLookSpeed, 0);
        }
        #endregion
    }
}