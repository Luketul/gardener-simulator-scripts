using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class WateringCan : MonoBehaviour
{
    public GameObject WateringCanPickedUp;
    private GameObject WateringCanToPickUp;
    private GameObject WateringCanPlace;
    public AudioSource WateringCanPickUp;
    public AudioSource WateringCanPutDown;
    public AudioSource WateringSound;
    public Transform playerCamera;
    public float pickupRange = 2f;
    public LayerMask interactableLayer;

    private PlayerMovement playerMovement;

    private Outline wateringCanOutline;
    private Outline wateringCanPlaceOutline;

    public ParticleSystem particlesystemWater;

    private Animator Watering;

    public bool isWatering = false; //flaga do animacji
    public bool isHoldingWateringCan = false;

    public float maxWaterAmount = 100f;
    public float currentWaterAmount;


    public Slider waterLevelSlider;
    public GameObject WateringCanOnSlider;

    public float waterConsumptionRate = 10f;

    public GameObject RefillPlace;
    public GameObject WateringCanAtRefillPlace;

    public bool outOfWater => currentWaterAmount <= 0;

    private Coroutine wateringSoundCoroutine;

    public GameObject dotcrosshair;

    private void Start()
    {
        WateringCanPickedUp.SetActive(false);
        particlesystemWater.Stop();
        playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();

        Watering = WateringCanPickedUp.GetComponent<Animator>();
        currentWaterAmount = maxWaterAmount;
        waterLevelSlider.maxValue = maxWaterAmount;
        waterLevelSlider.value = currentWaterAmount;
        waterLevelSlider.gameObject.SetActive(false);

        RefillPlace.SetActive(false);
        WateringCanAtRefillPlace.SetActive(false);

        WateringCanOnSlider.SetActive(false);
    }

    private void Update()
    {
        ShowInteractPrompt();
        HandleInteraction();

        if (isHoldingWateringCan && Input.GetMouseButtonDown(0) && currentWaterAmount > 0)
        {
            StartWateringPlants();
        }
        else if (isHoldingWateringCan && Input.GetMouseButton(0) && currentWaterAmount > 0)
        {
            if (isWatering)
            {
                ContinueWatering();
            }
        }
        else if (isHoldingWateringCan && Input.GetMouseButtonUp(0))
        {
            StopWateringPlants();
        }

        waterLevelSlider.value = currentWaterAmount;
    }

    public void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            if (hit.transform.gameObject == WateringCanToPickUp && !playerMovement.IsHoldingItem())
            {
                if (wateringCanOutline != null && !wateringCanOutline.enabled)
                {
                    wateringCanOutline.enabled = true;
                }
            }
            else
            {
                if (wateringCanOutline != null && wateringCanOutline.enabled)
                {
                    wateringCanOutline.enabled = false;
                }
            }
        }
        else
        {
            if (wateringCanOutline != null && wateringCanOutline.enabled)
            {
                wateringCanOutline.enabled = false;
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
                    if (hit.transform.gameObject == WateringCanToPickUp)
                    {
                        if (wateringCanOutline != null) wateringCanOutline.enabled = false;
                        PickUpWateringCan();
                    }
                }
                else if (hit.transform.gameObject == WateringCanPlace && playerMovement.heldItem == WateringCanPickedUp)
                {
                    DropWateringCan();
                }
            }
        }
    }

    public void PickUpWateringCan()
    {
        WateringCanPickedUp.SetActive(true);
        WateringCanPickUp.Play();
        playerMovement.PickupItem(WateringCanPickedUp);
        waterLevelSlider.gameObject.SetActive(true);
        isHoldingWateringCan = true;

        RefillPlace.SetActive(true);
        WateringCanAtRefillPlace.SetActive(false);

        dotcrosshair.SetActive(true);
        WateringCanOnSlider.SetActive(true);

    }

    public void DropWateringCan()
    {
        WateringCanPickedUp.SetActive(false);
        WateringCanPutDown.Play();
        playerMovement.DropItem();

        if (wateringCanPlaceOutline != null)
        {
            wateringCanPlaceOutline.enabled = false;
        }

        waterLevelSlider.gameObject.SetActive(false);
        isHoldingWateringCan = false;

        RefillPlace.SetActive(false);
        WateringCanAtRefillPlace.SetActive(false);


        StopCoroutine(wateringSoundCoroutine);

        dotcrosshair.SetActive(false);
        WateringCanOnSlider.SetActive(false);

    }

    public void PickUpWateringCanFromRefillStation()
    {
        WateringCanPickedUp.SetActive(true);

        WateringCanPickUp.Play();
        playerMovement.PickupItem(WateringCanPickedUp);

        waterLevelSlider.gameObject.SetActive(true);
        isHoldingWateringCan = true;

        RefillPlace.SetActive(true);
        WateringCanAtRefillPlace.SetActive(false);

        dotcrosshair.SetActive(true);
        WateringCanOnSlider.SetActive(true);

        particlesystemWater.Stop();
    }



    public void DropWateringCanToRefillStation()
    {
        WateringCanPickedUp.SetActive(false);
        WateringCanPutDown.Play();
        playerMovement.DropItem();

        if (wateringCanPlaceOutline != null)
        {
            wateringCanPlaceOutline.enabled = false;
        }

        waterLevelSlider.gameObject.SetActive(false);
        isHoldingWateringCan = false;

        RefillPlace.SetActive(false);
        WateringCanAtRefillPlace.SetActive(true);

        dotcrosshair.SetActive(false);
        WateringCanOnSlider.SetActive(false);

        if (wateringSoundCoroutine != null)
        {
            StopCoroutine(wateringSoundCoroutine);
        }
        wateringSoundCoroutine = StartCoroutine(FadeOutWateringSound()); 
    }



    private void StartWateringPlants()
    {

        if (wateringSoundCoroutine != null)
        {
            StopCoroutine(wateringSoundCoroutine);
        }

        wateringSoundCoroutine = StartCoroutine(FadeInWateringSound());

        if (Watering != null)
        {
            particlesystemWater.Play();
            Watering.SetBool("Watering", true);
            

        }

        isWatering = true;
    }

    private void ContinueWatering()
    {
        if (Watering != null)
        {
            Watering.SetBool("WateringStart", true);
            Watering.SetBool("Watering", false);
        }

        currentWaterAmount -= waterConsumptionRate * Time.deltaTime;
        if (currentWaterAmount <= 0)
        {
            currentWaterAmount = 0;
            StopWateringPlants();
        }
    }

    private void StopWateringPlants()
    {
        if (wateringSoundCoroutine != null)
        {
            StopCoroutine(wateringSoundCoroutine);
        }
        wateringSoundCoroutine = StartCoroutine(FadeOutWateringSound()); 

        if (Watering != null)
        {
            Watering.SetBool("WateringStart", false);
            particlesystemWater.Stop();
        }

        isWatering = false;
    }




    private IEnumerator FadeInWateringSound()
    {
        float duration = 0.1f; // Czas rozg³uszania
        float targetVolume = 0.35f; // Docelowa g³oœnoœæ

        if (WateringSound != null)
        {
            WateringSound.Play();
            while (WateringSound.volume < targetVolume)
            {
                WateringSound.volume += Time.deltaTime / duration;
                yield return null;
            }
            WateringSound.volume = targetVolume;
        }
    }

    private IEnumerator FadeOutWateringSound()
    {
        float duration = 0.9f; // Czas wyciszania

        if (WateringSound != null)
        {
            while (WateringSound.volume > 0f)
            {
                WateringSound.volume -= Time.deltaTime / duration;
                yield return null;
            }
            WateringSound.volume = 0f;
            WateringSound.Stop();
        }
    }
}
