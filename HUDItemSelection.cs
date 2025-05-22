using UnityEngine;

public class HUDItemSelection : MonoBehaviour
{
    public GameObject WateringCan;
    public GameObject TrashBag;
    public GameObject Sprayer;
    public GameObject Shears;
    public GameObject Gnome;


    private WateringCan wateringCanScript;
    private TrashBag TrashBagScript;
    private Sprayer sprayerScript;
    private Shears shearsScript;
    private Gnome gnomescript;

    private CarInteraction carInteraction;


    void Start()
    {
        if (WateringCan != null)
        {
            wateringCanScript = WateringCan.GetComponent<WateringCan>();
        }

        if (TrashBag != null)
        {
            TrashBagScript = TrashBag.GetComponent<TrashBag>();
        }

        if (Sprayer != null)
        {
            sprayerScript = Sprayer.GetComponent<Sprayer>();
        }

        if (Shears != null)
        {
            shearsScript = Shears.GetComponent<Shears>();
        }

        if (Gnome != null)
        {
            gnomescript = Gnome.GetComponent<Gnome>();
        }





        carInteraction = FindObjectOfType<CarInteraction>();
    }

    public void OnWateringCanSelected()
    {
        if (wateringCanScript != null)
        {
            wateringCanScript.PickUpWateringCan();

            if (carInteraction != null)
            {
                carInteraction.PickUpItem(WateringCan); 
                carInteraction.CloseHUD();
            }
        }
    }

    public void OnTrashBagSelected()
    {
        if (TrashBagScript != null)
        {
            TrashBagScript.PickUpTrashBag();

            if (carInteraction != null)
            {
                carInteraction.PickUpItem(TrashBag);
                carInteraction.CloseHUD();
            }
        }
    }

    public void OnGnomeSelected()
    {
        if (gnomescript != null)
        {
            gnomescript.PickUpGnome();

            if (carInteraction != null)
            {
                carInteraction.PickUpItem(Gnome);
                carInteraction.CloseHUD();
            }
        }
    }

    public void OnSprayerSelected()
    {
        if (sprayerScript != null)
        {
            sprayerScript.PickUpSprayer();

            if (carInteraction != null)
            {
                carInteraction.PickUpItem(Sprayer);
                carInteraction.CloseHUD();
            }
        }
    }

    public void OnShearsSelected()
    { 
        if (shearsScript != null)
        {
            shearsScript.PickUpShears();

            if (carInteraction != null)
            {
                carInteraction.PickUpItem(Shears);
                carInteraction.CloseHUD();
            }
        }
    }
}