using UnityEngine;
using System.Collections;

public class WaterRefillStation : MonoBehaviour
{
    public Transform playerCamera;
    public GameObject pressEtoInteractCanvas;
    public LayerMask interactableLayer;
    public WaterRefillPlace waterRefillPlaceScript;
    public WateringCan wateringCanScript;

    private Outline outline;
    private bool isPlayerLookingAtStation = false;
    public bool isRefilling = false;
    private float refillTime = 0.1f;
    private float refillTimer = 0f;

    public AudioSource GardenTapSound;
    public float maxDistance = 5f; 

    public ParticleSystem water;
    public Animator anim;


    //hints
    public Animator hint;

    private void Start()
    {
        water.Stop();
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        if (pressEtoInteractCanvas != null)
        {
            pressEtoInteractCanvas.SetActive(false);
        }

        GardenTapSound.Stop();
    }

    private void Update()
    {
        if (waterRefillPlaceScript == null || !waterRefillPlaceScript.WateringCanReadyToFill || wateringCanScript == null)
        {
            ResetInteraction();
            return;
        }

        if (isRefilling)
        {
            refillTimer += Time.deltaTime;
            if (refillTimer >= refillTime)
            {
                FinishRefilling();
            }
            return;
        }

        CheckPlayerLookingAtStation();
        AdjustAudioVolumeBasedOnDistance(); 
    }

    private void CheckPlayerLookingAtStation()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isPlayerLookingAtStation)
                {
                    isPlayerLookingAtStation = true;

                    if (outline != null) outline.enabled = true;
                    if (pressEtoInteractCanvas != null) pressEtoInteractCanvas.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartRefilling();
                }
            }
            else
            {
                ResetInteraction();
            }
        }
        else
        {
            ResetInteraction();
        }
    }

    private void ResetInteraction()
    {
        if (isPlayerLookingAtStation)
        {
            isPlayerLookingAtStation = false;

            if (outline != null) outline.enabled = false;
            if (pressEtoInteractCanvas != null) pressEtoInteractCanvas.SetActive(false);
        }
    }

    private void StartRefilling()
    {
        if (wateringCanScript.currentWaterAmount >= wateringCanScript.maxWaterAmount)
        {
            hint.SetBool("ISFULL", true);
            StartCoroutine(HideHintAfterDelay(2f));
            Debug.Log("Konewka jest ju¿ pe³na!");
            return;
        }

        anim.SetBool("Handle", true);
        Debug.Log("Rozpoczêto nape³nianie konewki...");
        isRefilling = true;
        refillTimer = 0f;
        water.Play();

        GardenTapSound.Play();

        waterRefillPlaceScript.WateringCanReadyToFill = false;

        StartCoroutine(RefillWater());
    }

    private IEnumerator RefillWater()
    {
        float refillDuration = 5f;

        while (refillTimer < refillDuration)
        {
            refillTimer += Time.deltaTime;
            yield return null;
        }

        FinishRefilling();
    }

    private void FinishRefilling()
    {
        anim.SetBool("Handle", false);
        GardenTapSound.Play(); 
        Debug.Log("Konewka zosta³a nape³niona!");
        isRefilling = false;

        wateringCanScript.currentWaterAmount = wateringCanScript.maxWaterAmount;
        wateringCanScript.waterLevelSlider.value = wateringCanScript.currentWaterAmount;

        waterRefillPlaceScript.WateringCanReadyToFill = true;

        if (!wateringCanScript.gameObject.activeInHierarchy)
        {
            wateringCanScript.gameObject.SetActive(true);  
        }
    }

    private void AdjustAudioVolumeBasedOnDistance()
    {
        if (GardenTapSound != null)
        {
            float distance = Vector3.Distance(playerCamera.position, transform.position);

            if (distance <= maxDistance)
            {
                GardenTapSound.volume = 0.7f - (distance / maxDistance);
            }
            else
            {
                GardenTapSound.volume = 0;
            }
        }
    }

    private IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hint.SetBool("ISFULL", false);
    }
}
