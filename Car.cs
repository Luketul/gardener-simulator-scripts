using Unity.VisualScripting;
using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public GameObject carHUD;
    public GameObject interactPrompt;
    public Transform playerCamera;
    public Transform player;
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    private Outline carOutline;
    private bool isLookingAtCar = false;

    private PlayerMovement playerMovement;

    public GameObject Cursorimage;

    private GameObject currentHeldItem;

    //scripts
    private PickUpItem pickupitemscript;
    private WateringCan wateringCanScript;
    private Shears shearsScript;
    private TrashBag playerTrashBag;
    private Sprayer SprayerScript;
    private Gnome gnomescript;

    public Animator radialMenuAnimator;

    public WaterRefillPlace waterRefillPlaceScript;
    public WaterRefillStation waterRefillStation;
    private GardenManager gardenManager;

    private bool hudOpen = false;



    void Start()
    {
        carOutline = GetComponent<Outline>();
        if (carOutline != null)
        {
            carOutline.enabled = false;
        }

        gardenManager = FindObjectOfType<GardenManager>();

        if (carHUD != null) carHUD.SetActive(false);
        if (interactPrompt != null) interactPrompt.SetActive(false);

        pickupitemscript = GetComponent<PickUpItem>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerTrashBag = FindObjectOfType<TrashBag>();
        waterRefillStation = FindObjectOfType<WaterRefillStation>();
        SprayerScript = FindObjectOfType<Sprayer>();
        gnomescript = FindObjectOfType<Gnome>();

    }

    void Update()
    {
        HandleOutlineAndPrompt();
        HandleInteraction();
        HandleItemDrop();
    }

    private void HandleOutlineAndPrompt()
    {
        if (playerTrashBag != null && playerTrashBag.isHoldingTrashBag)
        {
            ResetInteractionState();
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            if (hit.transform.gameObject == this.gameObject)
            {
                isLookingAtCar = true;
                if (interactPrompt != null) interactPrompt.SetActive(true);
                if (carOutline != null && !carOutline.enabled) carOutline.enabled = true;
            }
            else
            {
                ResetInteractionState();
            }
        }
        else
        {
            ResetInteractionState();
        }
    }

    private void ResetInteractionState()
    {
        isLookingAtCar = false;

        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (carOutline != null && carOutline.enabled) carOutline.enabled = false;
    }

    private void HandleInteraction()
    {
        if (pickupitemscript != null && pickupitemscript.isHeld)
        {
            return;
        }

        if (playerTrashBag != null && playerTrashBag.isHoldingTrashBag)
        {
            return;
        }

        if (isLookingAtCar && Input.GetKeyDown(KeyCode.E))
        {
            if (!hudOpen)
            {
                OpenHUD();
            }
            else
            {
                CloseHUD();
            }
        }
    }

    private void OpenHUD()
    {
        if (carHUD != null)
        {
            carHUD.SetActive(true);
        }

        if (radialMenuAnimator != null)
        {
            radialMenuAnimator.SetBool("Open", true);
            radialMenuAnimator.SetBool("Close", false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Cursorimage.SetActive(false);
        hudOpen = true;

    }

    public void CloseHUD()
    {
        if (radialMenuAnimator != null)
        {
            radialMenuAnimator.SetBool("Open", false);
            radialMenuAnimator.SetBool("Close", true);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Cursorimage.SetActive(true);
        }

        Invoke(nameof(DisableHUD), 0.5f);
        hudOpen = false;

    }

    private void DisableHUD()
    {
        if (carHUD != null)
        {
            carHUD.SetActive(false);
        }
    }

    private void HandleItemDrop()
    {
        if (isLookingAtCar && currentHeldItem != null && Input.GetKeyDown(KeyCode.E))
        {
            if (wateringCanScript != null && waterRefillPlaceScript.WateringCanReadyToFill == false && waterRefillStation.isRefilling == false)
            {
                wateringCanScript.DropWateringCan();
            }

            if (gnomescript != null)
            {
                gnomescript.DropGnome();
            }

            if (shearsScript != null)
            {
                shearsScript.DropShears();
            }

            if (SprayerScript != null)
            {
                SprayerScript.DropSprayer();
            }
            ResetHeldItem();
        }
    }


    public void PickUpItem(GameObject item)
    {
        currentHeldItem = item;

        wateringCanScript = item.GetComponent<WateringCan>();
        if (wateringCanScript != null)
        {
            wateringCanScript.PickUpWateringCan();
        }

        playerTrashBag = item.GetComponent<TrashBag>();
        if (playerTrashBag != null)
        {
            playerTrashBag.PickUpTrashBag();
        }

        shearsScript = item.GetComponent<Shears>();
        if (shearsScript != null)
        {
            shearsScript.PickUpShears();
        }

        SprayerScript = item.GetComponent<Sprayer>();
        if(SprayerScript != null)
        {
            SprayerScript.PickUpSprayer();
        }
        gnomescript = item.GetComponent<Gnome>();
        if (gnomescript != null)
        {
            gnomescript.PickUpGnome();
        }
    }

    private void ResetHeldItem()
    {
        currentHeldItem = null;
        wateringCanScript = null;
        shearsScript = null;
        SprayerScript = null;
        gnomescript=null;
    }
}
