using UnityEngine;

public class StickCutted : MonoBehaviour
{
    public GameObject stickPickedUp; 
    public GameObject stickToPickUp; 
    public AudioSource stickPickUpSound; 
    public Transform playerCamera; 
    public float pickupRange = 2f; 
    public LayerMask interactableLayer; 

    public GameObject pressEtoInteract; 

    private Outline stickOutline; 
    private PlayerMovement playerMovement; 

    public bool isHoldingStick = false; 
    public static StickCutted currentlyHeldStick = null; 

    private void Start()
    {
        stickPickedUp.SetActive(false); 
        stickToPickUp.SetActive(true); 
        pressEtoInteract.SetActive(false); 

        playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();

        stickOutline = stickToPickUp.GetComponent<Outline>();
        if (stickOutline != null)
        {
            stickOutline.enabled = false;
        }
    }

    private void Update()
    {
        ShowInteractPrompt();
        HandleInteraction();
    }

    private void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            if (hit.transform.gameObject == stickToPickUp && !playerMovement.IsHoldingItem())
            {
                pressEtoInteract.SetActive(true);

                if (stickOutline != null && !stickOutline.enabled)
                {
                    stickOutline.enabled = true;
                }
            }
            else
            {
                pressEtoInteract.SetActive(false);

                if (stickOutline != null && stickOutline.enabled)
                {
                    stickOutline.enabled = false;
                }
            }
        }
        else
        {
            pressEtoInteract.SetActive(false);

            if (stickOutline != null && stickOutline.enabled)
            {
                stickOutline.enabled = false;
            }
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
            {
                if (!playerMovement.IsHoldingItem() && hit.transform.gameObject == stickToPickUp)
                {
                    pressEtoInteract.SetActive(false);
                    if (stickOutline != null) stickOutline.enabled = false;
                    PickUpStick();
                }
            }
        }
    }

    private void PickUpStick()
    {
        stickToPickUp.SetActive(false);
        stickPickedUp.SetActive(true);

        stickPickedUp.transform.SetParent(playerCamera);
        stickPickedUp.transform.localPosition = new Vector3(0.15f, -0.24f, 0.48f);
        stickPickedUp.transform.localRotation = Quaternion.Euler(new Vector3(4f, -6.58f, -88.18f));


        Rigidbody stickRigidbody = stickToPickUp.GetComponent<Rigidbody>();
        if (stickRigidbody != null)
        {
            Destroy(stickRigidbody);
        }
        stickPickUpSound.Play();

        playerMovement.PickupItem(stickPickedUp);
        isHoldingStick = true;

        currentlyHeldStick = this;
    }

    public void DropStick()
    {
        stickPickedUp.SetActive(false);

        stickPickedUp.transform.SetParent(null);

        isHoldingStick = false;
        playerMovement.DropItem();

        currentlyHeldStick = null;
    }
}
