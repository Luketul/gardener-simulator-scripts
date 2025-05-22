using UnityEngine;

public class Sand : MonoBehaviour
{
    public Camera playerCamera;                   
    public LayerMask interactableLayer;           
    private Shovel shovelScript;                  

    private Outline sandOutline;                  
    private GameObject currentTarget;             

    void Start()
    {
        shovelScript = FindObjectOfType<Shovel>(); 

        sandOutline = GetComponent<Outline>();
        if (sandOutline != null)
            sandOutline.enabled = false;
    }

    void Update()
    {
        HandleOutline();                          
    }

    private void HandleOutline()
    {
        if (shovelScript.isHoldingShovel)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
            {
                if (hit.transform.gameObject != currentTarget)
                {
                    if (currentTarget != null && sandOutline != null)
                    {
                        sandOutline.enabled = false;
                    }

                    currentTarget = hit.transform.gameObject;
                    sandOutline = currentTarget.GetComponent<Outline>();

                    if (sandOutline != null)
                    {
                        sandOutline.enabled = true;
                    }
                }
            }
            else
            {
                if (currentTarget != null && sandOutline != null)
                {
                    sandOutline.enabled = false;
                }
                currentTarget = null;
            }
        }
        else
        {
            if (currentTarget != null && sandOutline != null)
            {
                sandOutline.enabled = false;
            }
        }
    }
}
