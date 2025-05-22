using UnityEngine;
using UnityEngine.UI;

public class HedgeTrimmer : MonoBehaviour
{
    public GameObject HedgeTrimmerPickedUp;
    public GameObject HedgeTrimmerToPickUp;
    public GameObject HedgeTrimmerPlace;
    public AudioSource HedgeTrimmerPickUp;
    public AudioSource HedgeTrimmerPutDown;
    public Transform playerCamera;
    public float pickupRange = 2f;
    public LayerMask interactableLayer;

    public AudioSource TrimmerInteraction;
    private PlayerMovement playerMovement;

    public GameObject pressetointerract;

    private Outline HedgeTrimmerOutline;
    private Outline HedgeTrimmerPlaceOutline;

    public bool isHoldingHedgeTrimmer = false;

    private void Start()
    {
        HedgeTrimmerPickedUp.SetActive(false);
        HedgeTrimmerToPickUp.SetActive(true);
        HedgeTrimmerPlace.SetActive(false);
        pressetointerract.SetActive(false);
        

        playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();

        HedgeTrimmerOutline = GetComponent<Outline>();
        if (HedgeTrimmerOutline != null)
        {
            HedgeTrimmerOutline.enabled = false;
        }

        HedgeTrimmerPlaceOutline = HedgeTrimmerPlace.GetComponent<Outline>();
        if (HedgeTrimmerPlaceOutline != null)
        {
            HedgeTrimmerPlaceOutline.enabled = false;
        }
    }

    private void Update()
    {
        ShowInteractPrompt();
        HandleInteraction();
        UpdateHedgeTrimmerPlaceOutline();
    }

    private void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            if (hit.transform.gameObject == HedgeTrimmerToPickUp && !playerMovement.IsHoldingItem())
            {
                pressetointerract.SetActive(true);

                if (HedgeTrimmerOutline != null && !HedgeTrimmerOutline.enabled)
                {
                    HedgeTrimmerOutline.enabled = true;
                }
            }
            else if (hit.transform.gameObject == HedgeTrimmerPlace && playerMovement.IsHoldingItem())
            {
                pressetointerract.SetActive(true);
            }
            else
            {
                pressetointerract.SetActive(false);

                if (HedgeTrimmerOutline != null && HedgeTrimmerOutline.enabled)
                {
                    HedgeTrimmerOutline.enabled = false;
                }
            }
        }
        else
        {
            pressetointerract.SetActive(false);

            if (HedgeTrimmerOutline != null && HedgeTrimmerOutline.enabled)
            {
                HedgeTrimmerOutline.enabled = false;
            }
        }
    }

    private void UpdateHedgeTrimmerPlaceOutline()
    {
        if (playerMovement.IsHoldingItem())
        {
            if (HedgeTrimmerPlaceOutline != null)
            {
                HedgeTrimmerPlaceOutline.enabled = true;
            }

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer) && hit.transform.gameObject == HedgeTrimmerPlace)
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
            if (HedgeTrimmerPlaceOutline != null)
            {
                HedgeTrimmerPlaceOutline.enabled = false;
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
                    if (hit.transform.gameObject == HedgeTrimmerToPickUp)
                    {
                        pressetointerract.SetActive(false);
                        if (HedgeTrimmerOutline != null) HedgeTrimmerOutline.enabled = false;
                        PickUpHedgeTrimmer();
                    }
                }
                else if (hit.transform.gameObject == HedgeTrimmerPlace && playerMovement.heldItem == HedgeTrimmerPickedUp)
                    {
                    pressetointerract.SetActive(false);
                    DropHedgeTrimmer();
                }
            }
        }
    }

    private void PickUpHedgeTrimmer()
    {
        HedgeTrimmerToPickUp.SetActive(false);
        HedgeTrimmerPickedUp.SetActive(true);
        HedgeTrimmerPlace.SetActive(true);
        HedgeTrimmerPickUp.Play();

        playerMovement.PickupItem(HedgeTrimmerPickedUp);
        isHoldingHedgeTrimmer = true;
    }

    private void DropHedgeTrimmer()
    {
        HedgeTrimmerPickedUp.SetActive(false);
        HedgeTrimmerToPickUp.SetActive(true);
        HedgeTrimmerPlace.SetActive(false);
        HedgeTrimmerPutDown.Play();

        playerMovement.DropItem();

        if (HedgeTrimmerPlaceOutline != null)
        {
            HedgeTrimmerPlaceOutline.enabled = false;
        }

        isHoldingHedgeTrimmer = false;
    }

    private void Trim()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrimmerInteraction.Play();
        }
    }
}
