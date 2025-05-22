using UnityEngine;
using UnityEngine.UI;


public class Gnome : MonoBehaviour
{
    public GameObject GnomeInHand;
    public GameObject GnomeToPut;
    public GameObject GnomeStandingPrefab; // Prefab trwałego krasnala

    public bool isHoldingGnome;

    public Transform playerCamera;

    public AudioSource PickUpSound;
    public AudioSource PutToCarSound;
    public AudioSource PlaceGnomeSound;

    public Material canPlaceMaterial;
    public Material cannotPlaceMaterial;

    private Renderer gnomeRenderer;
    private bool canPlaceHere = false;

    public float maxGnomeDistance = 2.5f;

    public Button actionButton;
    private GardenManager gardenManager;

    void Start()
    {
        gnomeRenderer = GnomeToPut.GetComponent<Renderer>();
        GnomeInHand.SetActive(false);
        GnomeToPut.SetActive(false);
        gardenManager = GardenManager.Instance;
    }

    void Update()
    {
        if (isHoldingGnome)
        {
            MoveGnomeToPut();
            if (Input.GetKeyDown(KeyCode.E) && canPlaceHere)
            {
                PlaceGnome();
            }
        }
    }

    private void MoveGnomeToPut()
    {
        if (GnomeToPut == null || playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxGnomeDistance))
        {
            GnomeToPut.transform.position = hit.point;


            canPlaceHere = hit.collider.CompareTag("Garden");
            UpdateGnomeMaterial(canPlaceHere);
        }
        else
        {
            canPlaceHere = false;
            UpdateGnomeMaterial(false);
        }
    }

    public void PickUpGnome()
    {
        GnomeInHand.SetActive(true);
        GnomeToPut.SetActive(true);
        PickUpSound.Play();
        isHoldingGnome = true;
    }

    public void DropGnome()
    {
        GnomeInHand.SetActive(false);
        GnomeToPut.SetActive(false);
        PutToCarSound.Play();
        isHoldingGnome = false;
    }

    public void PlaceGnome()
    {
        if (!canPlaceHere) return;

        Instantiate(GnomeStandingPrefab, GnomeToPut.transform.position, Quaternion.Euler(0, playerCamera.eulerAngles.y, 0));

        GnomeInHand.SetActive(false);
        GnomeToPut.SetActive(false);

        PlaceGnomeSound.Play();
        isHoldingGnome = false;

        actionButton.interactable = false; //button off

        GardenManager.Instance.GnomePlaced(); // Powiadom
    }




    private void UpdateGnomeMaterial(bool canPlace)
    {
        if (gnomeRenderer == null) return;
        {
            gnomeRenderer.material = canPlace ? canPlaceMaterial : cannotPlaceMaterial;
        }
    }
}
