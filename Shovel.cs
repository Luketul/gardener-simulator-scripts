using UnityEngine;
using UnityEngine.UI;

public class Shovel : MonoBehaviour
{
    public GameObject shovelPickedUp;
    public GameObject shovelToPickUp;
    public GameObject shovelPlace;
    public AudioSource shovelPickUp;
    public AudioSource shovelPutDown;
    public Transform playerCamera;
    public float pickupRange = 2f;
    public LayerMask interactableLayer;

    public AudioSource shovelInteraction;
    private PlayerMovement playerMovement;

    public GameObject pressetointerract;

    private Outline shovelOutline;
    private Outline shovelPlaceOutline;

    public bool isHoldingShovel = false;

    private void Start()
    {
        shovelPickedUp.SetActive(false);
        shovelToPickUp.SetActive(true);
        shovelPlace.SetActive(false);
        pressetointerract.SetActive(false);

        playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();

        shovelOutline = GetComponent<Outline>();
        if (shovelOutline != null)
        {
            shovelOutline.enabled = false;
        }

        shovelPlaceOutline = shovelPlace.GetComponent<Outline>();
        if (shovelPlaceOutline != null)
        {
            shovelPlaceOutline.enabled = false;
        }
    }

    private void Update()
    {
        ShowInteractPrompt();
        HandleInteraction();
        UpdateShovelPlaceOutline();
    }

    private void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            if (hit.transform.gameObject == shovelToPickUp && !playerMovement.IsHoldingItem())
            {
                pressetointerract.SetActive(true);

                if (shovelOutline != null && !shovelOutline.enabled)
                {
                    shovelOutline.enabled = true;
                }
            }
            else if (hit.transform.gameObject == shovelPlace && playerMovement.IsHoldingItem())
            {
                pressetointerract.SetActive(true);
            }
            else
            {
                pressetointerract.SetActive(false);

                if (shovelOutline != null && shovelOutline.enabled)
                {
                    shovelOutline.enabled = false;
                }
            }
        }
        else
        {
            pressetointerract.SetActive(false);

            if (shovelOutline != null && shovelOutline.enabled)
            {
                shovelOutline.enabled = false;
            }
        }
    }

    private void UpdateShovelPlaceOutline()
    {
        if (playerMovement.IsHoldingItem())
        {
            if (shovelPlaceOutline != null)
            {
                shovelPlaceOutline.enabled = true;
            }

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer) && hit.transform.gameObject == shovelPlace)
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
            if (shovelPlaceOutline != null)
            {
                shovelPlaceOutline.enabled = false;
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
                    if (hit.transform.gameObject == shovelToPickUp)
                    {
                        pressetointerract.SetActive(false);
                        if (shovelOutline != null) shovelOutline.enabled = false;
                        PickUpShovel();
                    }
                }
                else if (hit.transform.gameObject == shovelPlace && playerMovement.heldItem == shovelPickedUp)
                {
                    pressetointerract.SetActive(false);
                    DropShovel();
                }
            }
        }
    }

    private void PickUpShovel()
    {
        shovelToPickUp.SetActive(false);
        shovelPickedUp.SetActive(true);
        shovelPlace.SetActive(true);
        shovelPickUp.Play();

        playerMovement.PickupItem(shovelPickedUp);
        isHoldingShovel = true; 
    }

    private void DropShovel()
    {
        shovelPickedUp.SetActive(false);
        shovelToPickUp.SetActive(true);
        shovelPlace.SetActive(false);
        shovelPutDown.Play();

        playerMovement.DropItem();

        if (shovelPlaceOutline != null)
        {
            shovelPlaceOutline.enabled = false;
        }

        isHoldingShovel = false; 
    }

    private void Dig()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Lopata Kopie");
            shovelInteraction.Play();
        }
    }
}
