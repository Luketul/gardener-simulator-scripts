using UnityEngine;
using UnityEngine.UI;

public class WaterRefillPlace : MonoBehaviour
{
    public Transform playerCamera;
    public GameObject WateringCanWhereToPut;
    public GameObject PressEtoPut;
    public LayerMask interactableLayer;

    private WateringCan WateringCanScript;
    private TrashBag TrashBagScript;
    private Sprayer sprayerScript;
    private Outline outlinecan;

    public GameObject wateringCanPlacementPoint;

    public bool WateringCanReadyToFill;

    public Button actionButton;

    void Start()
    {
        WateringCanScript = FindObjectOfType<WateringCan>();
        TrashBagScript = FindObjectOfType<TrashBag>();
        sprayerScript = FindObjectOfType<Sprayer>();


        if (PressEtoPut != null) PressEtoPut.SetActive(false);
        if (WateringCanWhereToPut != null) WateringCanWhereToPut.SetActive(false);

        if (wateringCanPlacementPoint != null)
        {
            outlinecan = wateringCanPlacementPoint.GetComponent<Outline>();
            if (outlinecan != null) outlinecan.enabled = false;
        }
    }

    void Update()
    {
        if (WateringCanScript.isHoldingWateringCan)
        {
            WateringCanWhereToPut.SetActive(true);
            CheckPlayerLookingAtStation();
        }
        else
        {
            WateringCanWhereToPut.SetActive(false);
            if (PressEtoPut != null) PressEtoPut.SetActive(false);
            CheckPlayerLookingAtPickUpStation();
        }
    }

    private void CheckPlayerLookingAtStation()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
        {
            if (hit.collider.gameObject == WateringCanWhereToPut)
            {
                if (PressEtoPut != null) PressEtoPut.SetActive(true);
                if (outlinecan != null && !outlinecan.enabled) outlinecan.enabled = true;

                if (Input.GetKeyDown(KeyCode.E) && WateringCanScript.isHoldingWateringCan)
                {
                    PlaceWateringCan();
                }
            }
            else
            {
                ResetStationInteraction();
            }
        }
        else
        {
            ResetStationInteraction();
        }
    }

    private void CheckPlayerLookingAtPickUpStation()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
        {
            if (hit.collider.gameObject == wateringCanPlacementPoint && !WateringCanScript.isHoldingWateringCan
                && WateringCanReadyToFill && (TrashBagScript == null || !TrashBagScript.isHoldingTrashBag
                && (sprayerScript == null || !sprayerScript.isHoldingSprayer)))
            {
                if (PressEtoPut != null) PressEtoPut.SetActive(true);
                if (outlinecan != null && !outlinecan.enabled) outlinecan.enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpWateringCan();
                }
            }
            else
            {
                ResetStationInteraction();
            }
        }
        else
        {
            ResetStationInteraction();
        }
    }

    private void ResetStationInteraction()
    {
        if (PressEtoPut != null) PressEtoPut.SetActive(false);
        if (outlinecan != null && outlinecan.enabled) outlinecan.enabled = false;
    }

    private void PlaceWateringCan()
    {
        wateringCanPlacementPoint.SetActive(true);
        WateringCanScript.DropWateringCanToRefillStation();

        WateringCanReadyToFill = true;
        actionButton.interactable = false;
    }

    private void PickUpWateringCan()
    {
        if (TrashBagScript != null && TrashBagScript.isHoldingTrashBag) return;
        if (sprayerScript != null && sprayerScript.isHoldingSprayer) return;

        WateringCanScript.particlesystemWater.Stop();

        WateringCanScript.PickUpWateringCanFromRefillStation();

        wateringCanPlacementPoint.SetActive(false);

        WateringCanReadyToFill = false;
        actionButton.interactable = true;

        var carInteraction = FindObjectOfType<CarInteraction>();
        if (carInteraction != null)
        {
            carInteraction.PickUpItem(WateringCanScript.gameObject);
        }
    }
}
