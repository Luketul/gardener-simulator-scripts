using UnityEngine;

public class Trash : MonoBehaviour
{
    public GameObject trashToPickUp;
    public AudioSource trashPickUpSound;
    public Transform playerCamera;
    public float pickupRange = 2f;
    public LayerMask interactableLayer;
    public GameObject pressEtoInteract;

    private Outline trashOutline;
    private TrashBag trashBagScript;
    private static GameObject currentTarget; 
    private static Outline currentOutline;

    private void Start()
    {
        pressEtoInteract.SetActive(false);
        trashToPickUp.SetActive(true);

        trashOutline = trashToPickUp.GetComponent<Outline>();
        if (trashOutline != null)
        {
            trashOutline.enabled = false;
        }

        trashBagScript = FindObjectOfType<TrashBag>();
    }

    private void Update()
    {
        if (trashBagScript != null && trashBagScript.isHoldingTrashBag)
        {
            HandleOutline();
            HandleInteraction();
        }
    }

    private void HandleOutline()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            Trash hitTrash = hit.transform.GetComponent<Trash>();
            if (hitTrash != null)
            {
                if (currentTarget != hitTrash.gameObject)
                {
                    if (currentOutline != null)
                    {
                        currentOutline.enabled = false;
                        pressEtoInteract.SetActive(false);
                    }

                    currentTarget = hitTrash.gameObject;
                    currentOutline = hitTrash.trashOutline;

                    if (currentOutline != null)
                    {
                        currentOutline.enabled = true;
                        pressEtoInteract.SetActive(true);
                    }
                }
            }
        }
        else
        {
            ClearOutline();
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null)
        {
            Trash hitTrash = currentTarget.GetComponent<Trash>();
            if (hitTrash != null && trashBagScript != null)
            {
                if (trashBagScript.AddTrash())
                {
                    hitTrash.PickUpTrash();
                }
                else
                {
                    Debug.Log("Nie mo¿esz zebraæ wiêcej trash!");
                }
            }
        }
    }

    public void PickUpTrash()
    {
        trashToPickUp.SetActive(false);
        trashPickUpSound.Play();

        if (trashOutline != null)
        {
            trashOutline.enabled = false;
        }

        if (currentTarget == gameObject)
        {
            ClearOutline();
        }

        Destroy(gameObject);
    }

    private void ClearOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
        }
        pressEtoInteract.SetActive(false);
        currentTarget = null;
        currentOutline = null;
    }
}
