using UnityEngine;
using UnityEngine.UI;

public class Shears : MonoBehaviour
{
    public GameObject shearsPickedUp;
    public GameObject shearsToPickUp;
    public GameObject shearsPlace;
    public AudioSource shearsPickUp;
    public AudioSource shearsPutDown;
    public Transform playerCamera;
    public float pickupRange = 2f;
    public LayerMask interactableLayer;

    public AudioSource shearsInteraction;
    private PlayerMovement playerMovement;

    public GameObject pressetointerract;

    private Outline shearsOutline;
    private Outline shearsPlaceOutline;

    public Animator shears;

    public bool isHoldingShears = false;

    private void Start()
    {
        shearsPickedUp.SetActive(false);
        shearsToPickUp.SetActive(true);
        shearsPlace.SetActive(false);
        pressetointerract.SetActive(false);

        playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();

        shearsOutline = GetComponent<Outline>();
        if (shearsOutline != null)
        {
            shearsOutline.enabled = false;
        }

        shearsPlaceOutline = shearsPlace.GetComponent<Outline>();
        if (shearsPlaceOutline != null)
        {
            shearsPlaceOutline.enabled = false;
        }

        shears = shearsPickedUp.GetComponent<Animator>();
    }

    private void Update()
    {
        ShowInteractPrompt();
        HandleInteraction();
        UpdateShearsPlaceOutline();
        Cut();
    }

    private void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            if (hit.transform.gameObject == shearsToPickUp && !playerMovement.IsHoldingItem())
            {
                pressetointerract.SetActive(true);

                if (shearsOutline != null && !shearsOutline.enabled)
                {
                    shearsOutline.enabled = true;
                }
            }
            else if (hit.transform.gameObject == shearsPlace && playerMovement.IsHoldingItem())
            {
                pressetointerract.SetActive(true);
            }
            else
            {
                pressetointerract.SetActive(false);

                if (shearsOutline != null && shearsOutline.enabled)
                {
                    shearsOutline.enabled = false;
                }
            }
        }
        else
        {
            pressetointerract.SetActive(false);

            if (shearsOutline != null && shearsOutline.enabled)
            {
                shearsOutline.enabled = false;
            }
        }
    }

    private void UpdateShearsPlaceOutline()
    {
        if (playerMovement.IsHoldingItem())
        {
            if (shearsPlaceOutline != null)
            {
                shearsPlaceOutline.enabled = true;
            }

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer) && hit.transform.gameObject == shearsPlace)
            {
                pressetointerract.SetActive(true);
            }
            else
            {
                pressetointerract.SetActive(false);
            }
        }
        else
        {
            if (shearsPlaceOutline != null)
            {
                shearsPlaceOutline.enabled = false;
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
                if (!playerMovement.IsHoldingItem())
                {
                    if (hit.transform.gameObject == shearsToPickUp)
                    {
                        pressetointerract.SetActive(false);
                        if (shearsOutline != null) shearsOutline.enabled = false;
                        PickUpShears();
                    }
                }
                else if (hit.transform.gameObject == shearsPlace && playerMovement.heldItem == shearsPickedUp)
                {
                    pressetointerract.SetActive(false);
                    DropShears();
                }
            }
        }
    }

    public void PickUpShears()
    {
        shearsToPickUp.SetActive(false);
        shearsPickedUp.SetActive(true);
        shearsPlace.SetActive(true);
        shearsPickUp.Play();

        playerMovement.PickupItem(shearsPickedUp);
        isHoldingShears = true;
    }

    public void DropShears()
    {
        shearsPickedUp.SetActive(false);
        shearsToPickUp.SetActive(true);
        shearsPlace.SetActive(false);
        shearsPutDown.Play();

        playerMovement.DropItem();

        if (shearsPlaceOutline != null)
        {
            shearsPlaceOutline.enabled = false;
        }

        isHoldingShears = false;
    }

    private void Cut()
    {
        if (isHoldingShears && Input.GetMouseButtonDown(0)) // Gdy naciœniesz lewy przycisk myszy
        {
            // Odtworzenie dŸwiêku ciêcia
            shearsInteraction.Play();

            // Prze³¹czenie animacji no¿yc na "Cut"
            if (shears != null)
            {
                shears.SetTrigger("Cut");
            }
        }
        else if (isHoldingShears && !Input.GetMouseButtonDown(0)) // Gdy nie trzymasz przycisku myszy
        {
            // Prze³¹czenie animacji no¿yc na "Idle"
            if (shears != null)
            {
                shears.SetTrigger("Idle");
            }
        }
    }


}
