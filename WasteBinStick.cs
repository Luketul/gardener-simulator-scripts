using UnityEngine;

public class WasteBinStick : MonoBehaviour
{
    public Transform playerCamera;
    public LayerMask interactableLayer;
    public Animator Animator;
    private Outline wasteBinOutline;
    private GameObject currentTarget;

    public GameObject pressToInteract;

    void Start()
    {
        wasteBinOutline = GetComponent<Outline>();
        if (wasteBinOutline != null)
            wasteBinOutline.enabled = false;

        if (pressToInteract != null)
            pressToInteract.SetActive(false);
    }

    void Update()
    {
        HandleOutline();
        HandleInteraction();
    }

    private void HandleOutline()
    {
        if (StickCutted.currentlyHeldStick == null || !StickCutted.currentlyHeldStick.isHoldingStick)
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
        if (StickCutted.currentlyHeldStick != null && StickCutted.currentlyHeldStick.isHoldingStick && Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, interactableLayer) && hit.transform.gameObject == gameObject)
            {
                Animator.SetTrigger("WBanim");
                StickCutted.currentlyHeldStick.DropStick(); 
            }
        }
    }
}
