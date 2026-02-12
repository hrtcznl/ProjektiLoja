using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

[RequireComponent(typeof(CharacterController))] 
public class FPSController : MonoBehaviour 
{ 
    [Header("Camera & Look")] 
    public Camera playerCamera; 
    public float lookSpeed = 2f; 
    public float lookXLimit = 45f; 

    [Header("Movement")] 
    public float walkSpeed = 5f; 
    public float runSpeed = 10f; 
    public float jumpPower = 7f; 
    public float gravity = 10f; 
    public bool canMove = true; 

    [Header("Audio")] 
    public AudioSource walkingSound; 
    public AudioListener cameraListener; // assign the main camera listener here 
    public float audioDelay = 0.5f; // delay before enabling audio 

    private CharacterController characterController; 
    private Vector3 moveDirection = Vector3.zero; 
    private float rotationX = 0; 

    void Awake() 
    { 
        // Ensure walking sound doesn't play at start 
        walkingSound.playOnAwake = false; 
        walkingSound.Stop(); 

        // Make sure the AudioListener is initially disabled 
        if (cameraListener != null) 
            cameraListener.enabled = false; 
    } 

    void Start() 
    { 
        characterController = GetComponent<CharacterController>(); 
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 

        // Initialize rotationX to match the camera's current local X rotation
        rotationX = playerCamera.transform.localEulerAngles.x;
        if (rotationX > 180f)
            rotationX -= 360f;

        // Start coroutine to enable audio after a short delay
        StartCoroutine(EnableAudioAfterDelay()); 
    } 

    IEnumerator EnableAudioAfterDelay() 
    { 
        yield return new WaitForSeconds(audioDelay); 
        if (cameraListener != null) 
            cameraListener.enabled = true; 
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

        // Walking sound logic 
        bool moving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f; 

        if (moving && Input.GetButton("Jump") != true) 
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
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; 
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); 

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); 
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); 
        } 
        #endregion 
    } 
}
