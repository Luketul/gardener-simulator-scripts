using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WheelBarrowController : MonoBehaviour
{
    public Transform WheelBarrow;
    public Transform WheelBarrowCamera;
    public Transform playerCamera;
    public Transform player;
    public Transform playerSpawnPoint;
    public Transform WheelBarrowTarget;

    public float turnAngle = 30f;
    public float WheelBarrowSpeed = 5f;

    private CharacterController characterController;
    private MeshRenderer playerMeshRenderer;
    private bool isUsingWheelBarrow = false;
    private Quaternion lastPlayerRotation;
    private Vector3 lastPlayerPosition;

    public AudioSource Wheelbarrow;
    public Outline wheelbarrowOutline; 
    public TextMeshProUGUI interactHUD; 
    private PlayerMovement playerMovement;

    public GameObject Celownik;

    private void Start()
    {
        Celownik.SetActive(true);
        characterController = player.GetComponent<CharacterController>();
        playerMeshRenderer = player.GetComponent<MeshRenderer>();
        playerMovement = player.GetComponent<PlayerMovement>();

        WheelBarrowCamera.gameObject.SetActive(false);
        Wheelbarrow.Stop();

        if (wheelbarrowOutline != null)
        {
            wheelbarrowOutline.enabled = false; 
        }

        if (interactHUD != null)
        {
            interactHUD.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckForWheelBarrowInteraction();

        if (isUsingWheelBarrow)
        {
            ControlWheelBarrow();
        }

        ShowInteractPrompt(); 
    }

    private void CheckForWheelBarrowInteraction()
    {
        if (playerMovement.heldItem != null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isUsingWheelBarrow)
            {
                StopUsingWheelBarrow();
            }
            else
            {
                TryStartUsingWheelBarrow();
            }
        }
    }

    private void TryStartUsingWheelBarrow()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.transform == WheelBarrow)
            {
                StartUsingWheelBarrow();
            }
        }
    }

    public void StartUsingWheelBarrow()
    {
        Celownik.SetActive(false);

        if (wheelbarrowOutline != null) wheelbarrowOutline.enabled = false;
        interactHUD.gameObject.SetActive(false);

        playerCamera.GetComponent<AudioListener>().enabled = false;
        WheelBarrowCamera.GetComponent<AudioListener>().enabled = true;

        isUsingWheelBarrow = true;
        playerMovement.enabled = false;
        lastPlayerPosition = player.position;
        lastPlayerRotation = player.rotation;

        player.SetParent(WheelBarrow);

        playerCamera.gameObject.SetActive(false);
        WheelBarrowCamera.gameObject.SetActive(true);
        characterController.enabled = false;

        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = false;
        }

        Wheelbarrow.Play();
    }

    public void ControlWheelBarrow()
    {
        float moveZ = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        float rotationZ = turn * turnAngle * Time.deltaTime;
        WheelBarrow.Rotate(Vector3.forward * rotationZ, Space.Self);

        Vector3 moveDirection = WheelBarrow.up * moveZ;
        moveDirection.y = 0;

        WheelBarrow.Translate(moveDirection * WheelBarrowSpeed * Time.deltaTime, Space.World);

        if (Mathf.Abs(moveZ) > 0.1f || Mathf.Abs(turn) > 0.1f)
        {
            if (!Wheelbarrow.isPlaying)
            {
                Wheelbarrow.Play();
            }
        }
        else
        {
            Wheelbarrow.Stop();
        }
    }

    public void StopUsingWheelBarrow()
    {
        Celownik.SetActive(true);

        playerCamera.GetComponent<AudioListener>().enabled = true;
        WheelBarrowCamera.GetComponent<AudioListener>().enabled = false;

        isUsingWheelBarrow = false;
        Wheelbarrow.Stop();
        playerMovement.enabled = true;
        player.SetParent(null);

        if (playerSpawnPoint != null)
        {
            player.position = playerSpawnPoint.position;

            if (WheelBarrowTarget != null)
            {
                Vector3 directionToTarget = WheelBarrowTarget.position - player.position;
                directionToTarget.y = 0;

                player.rotation = Quaternion.LookRotation(directionToTarget);
            }
        }

        WheelBarrowCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        characterController.enabled = true;

        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = true;
        }
    }

    private void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f) && hit.transform == WheelBarrow && !isUsingWheelBarrow)
        {
            interactHUD.gameObject.SetActive(true);
            interactHUD.text = "Press E to Interact";
            if (wheelbarrowOutline != null) wheelbarrowOutline.enabled = true;
        }
        else
        {
            interactHUD.gameObject.SetActive(false);
            if (wheelbarrowOutline != null) wheelbarrowOutline.enabled = false;
        }
    }
}
