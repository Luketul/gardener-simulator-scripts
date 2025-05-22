using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public Transform PlayerCamera;
    public float pickupRange = 2f;
    public LayerMask interactableLayer;

    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private Outline outline;

    public Transform holdPosition;
    public float throwForce = 500f;

    public bool isHeld;
    public float rotationSpeed = 50f;
    private bool inSandbox = false;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    private void Update()
    {
        ShowOutline();
        HandleInteraction();

        if (isHeld)
        {
            RotateObject();

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropObject();
            }
        }
    }

    private void PickUpObject()
    {
        if (playerMovement.IsHoldingItem()) return;

        isHeld = true;
        playerMovement.PickupItem(gameObject);
        rb.isKinematic = true;
        transform.position = holdPosition.position;
        transform.parent = holdPosition;

        gameObject.layer = LayerMask.NameToLayer("HeldItem");

        if (outline != null) outline.enabled = false;
    }

    private void DropObject()
    {
        isHeld = false;
        playerMovement.DropItem();
        transform.parent = null;
        rb.isKinematic = false;

        gameObject.layer = LayerMask.NameToLayer("Interactable");

        if (inSandbox)
        {
            Debug.Log("Przedmiot wrzucony do piaskownicy");
            rb.isKinematic = true;
            rotationSpeed = 0f;
        }
    }

    private void ShowOutline()
    {
        Ray ray = new Ray(PlayerCamera.position, PlayerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            if (hit.collider.gameObject == gameObject && !playerMovement.IsHoldingItem())
            {
                if (outline != null) outline.enabled = true;
            }
            else
            {
                if (outline != null) outline.enabled = false;
            }
        }
        else
        {
            if (outline != null) outline.enabled = false;
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(PlayerCamera.position, PlayerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
            {
                if (!isHeld && hit.collider.gameObject == gameObject)
                {
                    PickUpObject();
                }
                else if (isHeld)
                {
                    DropObject();
                }
            }
        }
    }

    private void RotateObject()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sandbox"))
        {
            Debug.Log("Przedmiot wrzucony do piaskownicy");
            inSandbox = true;
            GardenManager.Instance.AddToy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sandbox"))
        {
            Debug.Log("Przedmiot wyjęty z piaskownicy");
            inSandbox = false;
            GardenManager.Instance.SubtractToy();
        }
    }
}
