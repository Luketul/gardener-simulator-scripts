using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    private float currentSpeed;
    public AudioClip[] grassFootsteps;
    public AudioClip[] pavementFootsteps;
    public AudioSource RunningKeysSound;
    public float walkFootstepInterval = 0.5f;
    public float sprintFootstepInterval = 0.3f;
    private float footstepTimer = 0f;

    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    public float pickupRange = 3f;
    private CharacterController characterController;


    public GameObject heldItem = null;
    public bool itemPickedUp = false;


    public bool isControllingLawnMower = false;
    public bool isControllingWheelBarrow = false;

    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;

    public GameObject Celownik;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Celownik.SetActive(true);
        RunningKeysSound.Stop();
        RunningKeysSound.loop = true;
    }

    private void Update()
    {
        if (!isControllingLawnMower && !isControllingWheelBarrow && characterController.enabled)
        {
            MovePlayer();
            RotatePlayer();
            CheckFootsteps();
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
            if (!RunningKeysSound.isPlaying)
            {
                RunningKeysSound.Play();
            }
        }
        else
        {
            currentSpeed = walkSpeed;
            if (RunningKeysSound.isPlaying)
            {
                RunningKeysSound.Stop();
            }
        }

        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckFootsteps()
    {
        float stepInterval = Input.GetKey(KeyCode.LeftShift) ? sprintFootstepInterval : walkFootstepInterval;

        if (characterController.velocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstepBasedOnSurface();
                footstepTimer = stepInterval;
            }
        }
        else
        {
            footstepTimer = stepInterval;
        }
    }

    private void PlayFootstepBasedOnSurface()
    {
        AudioClip[] selectedClips = pavementFootsteps;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance + 1f, groundMask))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                selectedClips = grassFootsteps;
            }
        }

        if (selectedClips.Length > 0)
        {
            int randomIndex = Random.Range(0, selectedClips.Length);
            AudioSource.PlayClipAtPoint(selectedClips[randomIndex], transform.position);
        }
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }

    public void PickupItem(GameObject item)
    {
        heldItem = item;
    }

    public void DropItem()
    {
        heldItem = null;
    }
}