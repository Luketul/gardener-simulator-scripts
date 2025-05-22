using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sprayer : MonoBehaviour
{
    public GameObject SprayerPickedUp;

    public AudioSource SprayerPickUp;
    public AudioSource SprayerPutDown;
    public Transform playerCamera;
    public float pickupRange = 3f;
    public LayerMask interactableLayer;
    public ParticleSystem particleSpray;
    public Animator spray;
    public AudioSource SprayerInteraction;
    private PlayerMovement playerMovement;

    public bool isHoldingSprayer;
    public GameObject Celownik;

    private void Start()
    {

        playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();
        SprayerPickedUp.SetActive(false);
        particleSpray.Stop();
        Celownik.SetActive(false);
    }

    private void Update()
    {
        Spray();
    }

    public void Spray()
    {
        if(isHoldingSprayer  && Input.GetMouseButtonDown(0))
        {
            spray.SetTrigger("Spray");
            SprayerInteraction.Play();
            particleSpray.Play();
        }
    }

    public void PickUpSprayer()
    {
        SprayerPickedUp.SetActive(true);
        SprayerPickUp.Play();
        isHoldingSprayer = true;

        playerMovement.PickupItem(SprayerPickedUp);
        Celownik.SetActive(true);
    }

    public void DropSprayer()
    {
        SprayerPickedUp.SetActive(false);
        SprayerPutDown.Play();
        isHoldingSprayer = false;

        playerMovement.DropItem();
        Celownik.SetActive(false);
    }
}
