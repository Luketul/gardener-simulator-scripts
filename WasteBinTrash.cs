using UnityEngine;

public class WasteBinTrash : MonoBehaviour
{
    public Transform playerCamera;
    public LayerMask interactableLayer;
    public Animator Animator;
    private Outline wasteBinOutline;
    private GameObject currentTarget;

    public GameObject pressToInteract;
    public ParticleSystem dust;

    private TrashBag playerTrashBag;

    void Start()
    {
        wasteBinOutline = GetComponent<Outline>();
        if (wasteBinOutline != null)
            wasteBinOutline.enabled = false;

        if (pressToInteract != null)
            pressToInteract.SetActive(false);

        dust.Stop();

        playerTrashBag = FindObjectOfType<TrashBag>();
    }

    void Update()
    {
        HandleOutline();
        HandleInteraction();
    }

    private void HandleOutline()
    {
        if (playerTrashBag == null || !playerTrashBag.isHoldingTrashBag)
        {
            if (currentTarget != null && wasteBinOutline != null)
            {
                wasteBinOutline.enabled = false;
            }

            if (pressToInteract != null)
                pressToInteract.SetActive(false);

            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
        {
            if (hit.transform.gameObject != currentTarget)
            {
                if (currentTarget != null && wasteBinOutline != null)
                {
                    wasteBinOutline.enabled = false;
                }

                currentTarget = hit.transform.gameObject;
                wasteBinOutline = currentTarget.GetComponent<Outline>();

                if (wasteBinOutline != null)
                {
                    wasteBinOutline.enabled = true;
                }

                if (pressToInteract != null)
                    pressToInteract.SetActive(true);
            }
        }
        else
        {
            if (currentTarget != null && wasteBinOutline != null)
            {
                wasteBinOutline.enabled = false;
            }

            currentTarget = null;

            if (pressToInteract != null)
                pressToInteract.SetActive(false);
        }
    }

    private void HandleInteraction()
    {
        // Usuniêto sprawdzanie liczby œmieci - worek mo¿na wyrzuciæ nawet pusty
        if (playerTrashBag != null && playerTrashBag.isHoldingTrashBag && Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, interactableLayer) && hit.transform.gameObject == gameObject)
            {
                Animator.SetTrigger("WBanim");

                int trashToAdd = playerTrashBag.GetTrashCount(); // Pobierz iloœæ œmieci w worku
                GardenManager.Instance.AddTrash(trashToAdd); // Dodaj punkty, nawet jeœli to 0

                playerTrashBag.DropTrashBag(); // Opró¿nij worek
                dust.Play(); // Efekt cz¹steczek
            }
        }
    }
}
