using UnityEngine;

public class Hedge: MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask interactableLayer;
    private HedgeTrimmer HedgeTrimmerScript;

    private Outline hedgeOutline;
    private GameObject currentTarget;

    public GameObject Pressetointerract;

    void Start()
    {
        Pressetointerract.SetActive(false);
        HedgeTrimmerScript = FindObjectOfType<HedgeTrimmer>();

        hedgeOutline = GetComponent<Outline>();
        if (hedgeOutline != null)
            hedgeOutline.enabled = false;
    }

    void Update()
    {
        HandleOutline();
    }

    private void HandleOutline()
    {
        if (HedgeTrimmerScript.isHoldingHedgeTrimmer)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
            {
                if (hit.transform.gameObject != currentTarget)
                {
                    if (currentTarget != null && hedgeOutline != null)
                    {
                        hedgeOutline.enabled = false;
                        Pressetointerract.SetActive(false);
                    }

                    currentTarget = hit.transform.gameObject;
                    hedgeOutline = currentTarget.GetComponent<Outline>();

                    if (hedgeOutline != null)
                    {
                        hedgeOutline.enabled = true;
                        Pressetointerract.SetActive(true);
                    }
                }
            }
            else
            {
                if (currentTarget != null && hedgeOutline != null)
                {
                    hedgeOutline.enabled = false;
                    Pressetointerract.SetActive(false);
                }
                currentTarget = null;
            }
        }
        else
        {
            if (currentTarget != null && hedgeOutline != null)
            {
                hedgeOutline.enabled = false;
                Pressetointerract.SetActive(false);
            }
        }
    }
}
