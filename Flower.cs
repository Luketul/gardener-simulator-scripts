using UnityEngine;
using UnityEngine.UI;

public class Flowers : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask interactableLayer;
    private WateringCan wateringCanScript;
    private Sprayer sprayerScript;
    private Outline flowerOutline;


    //watering
    private bool isWatered = false;
    private float wateringTime = 3f;
    private float currentWateringTime = 0f;
    private bool isPlayerLookingAtThisFlower = false;
    public GameObject wateringFeedbackText;
    public Slider wateringSlider;
    public ParticleSystem wateringParticles;
    public AudioSource WaterSuccess;

    //sprayer
    public AudioSource SpraySuccess;
    private bool isSprayed = false;
    private int sprayCount = 0;
    private int requiredSprays = 4;
    public ParticleSystem piana;
    public Slider spraySlider;


    public ParticleSystem successParticles;


    private void Start()
    {
        sprayerScript = FindObjectOfType<Sprayer>();
        wateringCanScript = FindObjectOfType<WateringCan>();

        if (wateringFeedbackText != null)
            wateringFeedbackText.SetActive(false);

        flowerOutline = GetComponent<Outline>();
        if (flowerOutline != null)
            flowerOutline.enabled = false; 

        if (wateringSlider != null)
        {
            wateringSlider.gameObject.SetActive(false);  
            wateringSlider.value = 0f;
        }

        if (wateringParticles != null)
            wateringParticles.Stop();  

        if (successParticles != null)
            successParticles.Stop();

        //sprayer
        piana.Stop();

        if (spraySlider != null)
        {
            spraySlider.gameObject.SetActive(false);
            spraySlider.value = 0f;
        }

    }

    private void Update()
    {
        if (wateringCanScript.isHoldingWateringCan && !wateringCanScript.outOfWater)
        {
            HandlePlayerLooking();
            HandleWatering();
        }
        else if (sprayerScript.isHoldingSprayer && isWatered && !isSprayed)
        {
            HandlePlayerLookingSpraying();
            HandleSpraying();
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
                if (!isPlayerLookingAtThisFlower)
                {
                    isPlayerLookingAtThisFlower = true;
                    if (flowerOutline != null)
                        flowerOutline.enabled = true;  

                    if (isWatered && wateringFeedbackText != null)
                        wateringFeedbackText.SetActive(true);

                    if (wateringSlider != null && !wateringSlider.gameObject.activeSelf && !isWatered)
                    {
                        wateringSlider.gameObject.SetActive(true);  
                    }
                }
            }
            else
            {
                ResetLookingState(); 
            }
        }
        else
        {
            ResetLookingState();  
        }
    }

    private void ResetLookingState()
    {
        if (isPlayerLookingAtThisFlower)
        {
            isPlayerLookingAtThisFlower = false;

            if (flowerOutline != null)
                flowerOutline.enabled = false; 

            if (wateringFeedbackText != null)
                wateringFeedbackText.SetActive(false);

            if (wateringSlider != null && wateringSlider.gameObject.activeSelf)
            {
                wateringSlider.gameObject.SetActive(false);  
            }

            if (wateringParticles != null && wateringParticles.isPlaying)
            {
                wateringParticles.Stop(); 
            }

            if (spraySlider != null && spraySlider.gameObject.activeSelf)
            {
                spraySlider.gameObject.SetActive(false);
            }
        }
    }

    private void HandleWatering()
    {
        if (isPlayerLookingAtThisFlower && !isWatered)
        {
            if (Input.GetMouseButton(0)) 
            {
                if (wateringCanScript.currentWaterAmount > 0)  
                {
                    currentWateringTime += Time.deltaTime;

                    if (wateringSlider != null)
                        wateringSlider.value = currentWateringTime / wateringTime;  

                    if (wateringParticles != null && !wateringParticles.isPlaying)
                    {
                        wateringParticles.Play(); 
                    }

                    if (currentWateringTime >= wateringTime)
                    {
                        WaterTheFlower();  
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
                    wateringSlider.value = 0f;  

                if (wateringParticles != null && wateringParticles.isPlaying)
                {
                    wateringParticles.Stop(); 
                }
            }
        }
    }

    private void WaterTheFlower()
    {
        isWatered = true;
        currentWateringTime = 0f;

        if (wateringFeedbackText != null)
            wateringFeedbackText.SetActive(true);

        if (successParticles != null)
        {
            successParticles.Play();
        }

        if (wateringParticles != null && wateringParticles.isPlaying)
        {
            wateringParticles.Stop();
        }

        if (WaterSuccess != null)
        {
            WaterSuccess.Play();
        }

        if (wateringSlider != null)
        {
            wateringSlider.gameObject.SetActive(false); 
        }

        GardenManager.Instance.AddWateredFlower();
        Debug.Log($"{gameObject.name} has been watered!");
    }


    private void HandleSpraying()
    {
        if (isPlayerLookingAtThisFlower && !isSprayed)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                sprayCount++;
                Debug.Log($"Sprayed {sprayCount}/{requiredSprays} times on {gameObject.name}");

                piana.Play();

                if (spraySlider != null)
                    spraySlider.value = (float)sprayCount / requiredSprays; 

                if (sprayCount >= requiredSprays)
                {
                    SprayTheFlower();
                }
            }
        }
    }


    private void HandlePlayerLookingSpraying()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
        {
            if (hit.transform == transform)
            {
                if (!isPlayerLookingAtThisFlower)
                {
                    isPlayerLookingAtThisFlower = true;
                    if (flowerOutline != null)
                        flowerOutline.enabled = true;  

                    if (spraySlider != null && !isSprayed)
                    {
                        spraySlider.gameObject.SetActive(true);
                        spraySlider.value = (float)sprayCount / requiredSprays; 
                    }
                }
            }
            else
            {
                ResetLookingState();
            }
        }
        else
        {
            ResetLookingState();
        }
    }





    private void SprayTheFlower()
    {
        SpraySuccess.Play();
        successParticles.Play();
        isSprayed = true;
        GardenManager.Instance.AddSprayedFlower();
        Debug.Log($"{gameObject.name} has been fully sprayed!");

        if (spraySlider != null)
            spraySlider.gameObject.SetActive(false); 
    }


}
