using UnityEngine;
using UnityEngine.UI;

public class Bush : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask interactableLayer;
    private WateringCan wateringCanScript;

    private Outline bushOutline;
    private bool isWatered = false;
    private float wateringTime = 3f;
    private float currentWateringTime = 0f;
    private bool isPlayerLookingAtThisBush = false;

    public GameObject wateringFeedbackText;
    public Slider wateringSlider;
    public ParticleSystem wateringParticles;
    public ParticleSystem successParticles;
    public AudioSource WaterSuccess;

    private void Start()
    {
        wateringCanScript = FindObjectOfType<WateringCan>();

        if (wateringFeedbackText != null)
            wateringFeedbackText.SetActive(false);

        bushOutline = GetComponent<Outline>();
        if (bushOutline != null)
            bushOutline.enabled = false;  // Outline is disabled initially

        if (wateringSlider != null)
        {
            wateringSlider.gameObject.SetActive(false);  // Slider is hidden initially
            wateringSlider.value = 0f;
        }

        if (wateringParticles != null)
            wateringParticles.Stop();  // Stop particles initially

        if (successParticles != null)
            successParticles.Stop();  // Stop success particles initially
    }

    private void Update()
    {
        if (wateringCanScript.isHoldingWateringCan && !wateringCanScript.outOfWater)
        {
            HandlePlayerLooking();
            HandleWatering();
        }
        else
        {
            ResetLookingState();
        }
    }

    private void HandlePlayerLooking()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
        {
            if (hit.transform == transform)
            {
                if (!isPlayerLookingAtThisBush)
                {
                    isPlayerLookingAtThisBush = true;
                    if (bushOutline != null)
                        bushOutline.enabled = true;  // Enable outline when looking at the bush

                    if (isWatered && wateringFeedbackText != null)
                        wateringFeedbackText.SetActive(true);

                    if (wateringSlider != null && !wateringSlider.gameObject.activeSelf && !isWatered)
                    {
                        wateringSlider.gameObject.SetActive(true);  // Show slider if bush is not watered
                    }
                }
            }
            else
            {
                ResetLookingState();  // Reset state if not looking at the bush
            }
        }
        else
        {
            ResetLookingState();  // Reset state if raycast doesn't hit the object
        }
    }

    private void ResetLookingState()
    {
        if (isPlayerLookingAtThisBush)
        {
            isPlayerLookingAtThisBush = false;

            if (bushOutline != null)
                bushOutline.enabled = false;  // Disable outline

            if (wateringFeedbackText != null)
                wateringFeedbackText.SetActive(false);

            if (wateringSlider != null && wateringSlider.gameObject.activeSelf)
            {
                wateringSlider.gameObject.SetActive(false);  // Hide slider when not looking
            }

            if (wateringParticles != null && wateringParticles.isPlaying)
            {
                wateringParticles.Stop();  // Stop particles when not looking
            }
        }
    }

    private void HandleWatering()
    {
        if (isPlayerLookingAtThisBush && !isWatered)
        {
            if (Input.GetMouseButton(0))  // Left mouse button
            {
                if (wateringCanScript.currentWaterAmount > 0)  // Check if watering can has water
                {
                    currentWateringTime += Time.deltaTime;

                    if (wateringSlider != null)
                        wateringSlider.value = currentWateringTime / wateringTime;  // Update slider

                    if (wateringParticles != null && !wateringParticles.isPlaying)
                    {
                        wateringParticles.Play();  // Play particles while watering
                    }

                    if (currentWateringTime >= wateringTime)
                    {
                        WaterTheBush();  // Finish watering
                    }
                }
                else
                {
                    Debug.Log("Out of water!");
                }
            }
            else
            {
                currentWateringTime = 0f;

                if (wateringSlider != null)
                    wateringSlider.value = 0f;  // Reset slider if button is not held

                if (wateringParticles != null && wateringParticles.isPlaying)
                {
                    wateringParticles.Stop();  // Stop particles when button is released
                }
            }
        }
    }

    private void WaterTheBush()
    {
        isWatered = true;
        currentWateringTime = 0f;

        if (wateringFeedbackText != null)
            wateringFeedbackText.SetActive(true);

        // Play success particles
        if (successParticles != null)
        {
            successParticles.Play();
        }

        // Stop watering particles
        if (wateringParticles != null && wateringParticles.isPlaying)
        {
            wateringParticles.Stop();
        }

        // Play success sound
        if (WaterSuccess != null)
        {
            WaterSuccess.Play();
        }

        // Hide slider after watering
        if (wateringSlider != null)
        {
            wateringSlider.gameObject.SetActive(false);  // Hide slider after watering
        }

        GardenManager.Instance.AddWateredBush();
        Debug.Log($"{gameObject.name} has been watered!");
    }
}
