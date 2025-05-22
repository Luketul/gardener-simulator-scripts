using UnityEngine;

public class Radio : MonoBehaviour
{
    public GameObject radio; 
    public Transform playerTransform; 
    public float maxDistance = 5f; 
    public AudioSource audioSource; 
    public AudioSource switchh;

    public float interactRange = 2f;
    public Transform playerCamera; 
    public LayerMask interactableLayer; 
    public GameObject pressEtoInteract; 

    private Outline radioOutline; 
    public bool isPlaying = true;

    public GameObject lightRed;
    public GameObject lightGreen;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = radio.GetComponent<AudioSource>();
        }

        if (pressEtoInteract != null)
        {
            pressEtoInteract.SetActive(false);
        }

        radioOutline = radio.GetComponent<Outline>();
        if (radioOutline != null)
        {
            radioOutline.enabled = false; 
        }

        if (audioSource != null)
        {
            audioSource.loop = true; 
            audioSource.volume = 0.4f;
            audioSource.Play(); 
        }

        lightGreen.SetActive(true);
        lightRed.SetActive(false);
    }

    void Update()
    {
        HandleOutlineAndInteraction();

        if (audioSource != null && isPlaying)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);

            if (distance <= maxDistance)
            {
                audioSource.volume = 0.4f - (distance / maxDistance); 
            }
            else
            {
                audioSource.volume = 0; 
            }
        }
    }

    private void HandleOutlineAndInteraction()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer) && hit.transform.gameObject == radio)
        {
            if (pressEtoInteract != null)
            {
                pressEtoInteract.SetActive(true);
            }

            if (radioOutline != null)
            {
                radioOutline.enabled = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleRadio();
            }
        }
        else
        {
            if (pressEtoInteract != null)
            {
                pressEtoInteract.SetActive(false);
            }

            if (radioOutline != null)
            {
                radioOutline.enabled = false;
            }
        }
    }

    private void ToggleRadio()
    {
     

        if (audioSource == null) return;

        isPlaying = !isPlaying;

        if (isPlaying)
        {
            switchh.Play();
            audioSource.Play();
            lightGreen.SetActive(true);
            lightRed.SetActive(false);
        }
        else
        {
            switchh.Play();
            audioSource.Stop();
            lightGreen.SetActive(false);
            lightRed.SetActive(true);
        }
    }
}
